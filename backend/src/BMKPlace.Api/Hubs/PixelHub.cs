using BMKPlace.Application.Contracts.Abstractions.Realtime;
using BMKPlace.Application.Contracts.DTOs.Common;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using BMKPlace.Application.Features.PixelCanvas.Queries.GetCanvasState;
using BMKPlace.Application.Features.Pixels.Commands.PlacePixelHub;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SharedKernel.Exceptions;
using SharedKernel.Helpers.Canvas;
using System.Security.Claims;

namespace BMKPlace.Api.Hubs;

[Authorize]
public class PixelHub : Hub<IPixelHubClient>, IPixelHub
{
    private readonly IMediator _mediator;
    private readonly ILogger<PixelHub> _logger;
    private readonly IPixelNotifier _pixelNotifier;
    private const string CanvasIdItemKey = "CanvasId";

    public PixelHub(IMediator mediator, ILogger<PixelHub> logger, IPixelNotifier pixelNotifier)
    {
        _mediator = mediator;
        _logger = logger;
        _pixelNotifier = pixelNotifier;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        string connectionId = Context.ConnectionId;
        string? userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";
        _logger.LogInformation("Client connected: {ConnectionId}, User: {UserId}", connectionId, userId);

        HttpContext? httpContext = Context.GetHttpContext();
        string? canvasIdString = httpContext?.Request.Query["canvasId"];

        if (string.IsNullOrWhiteSpace(canvasIdString) || !int.TryParse(canvasIdString, out int canvasId) || canvasId <= 0)
        {
            _logger.LogWarning("Client {ConnectionId} connected without a valid 'canvasId' query string parameter. Disconnecting.", connectionId);
            Context.Abort(); 
            return;
        }

        Context.Items[CanvasIdItemKey] = canvasId;
        string groupName = CanvasHelpers.GetCanvasGroupName(canvasId);

        await Groups.AddToGroupAsync(connectionId, groupName, Context.ConnectionAborted);
        _logger.LogInformation("Client {ConnectionId} added to group '{GroupName}' for Canvas {CanvasId}.", connectionId, groupName, canvasId);


        _logger.LogInformation("Client {ConnectionId} requested initial state for Canvas {CanvasId}.", connectionId, canvasId);
        try
        {
            var query = new GetCanvasStateQuery { CanvasId = canvasId, IgnoreCache = false };
            var canvasState = await _mediator.Send(query);

            if (canvasState != null)
            {
                await Clients.Caller.ReceiveInitialCanvasState(canvasState);
                _logger.LogInformation("Successfully sent initial state for Canvas {CanvasId} to Client {ConnectionId}.", canvasId, connectionId);
            }
            else
            {
                 _logger.LogWarning("GetCanvasStateQuery returned null for Canvas {CanvasId}. No initial state sent to Client {ConnectionId}.", canvasId, connectionId);
                 await Clients.Caller.ReceiveError(new ErrorDto("NotFound", "Canvas State Error", 404, $"Could not retrieve state for Canvas {canvasId}."));
            }
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Canvas {CanvasId} not found when fetching initial state for Client {ConnectionId}.", canvasId, connectionId);
            await Clients.Caller.ReceiveError(new ErrorDto("NotFound", "Canvas Not Found", 404, ex.Message));
            Context.Abort(); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching or sending initial state for Canvas {CanvasId} to Client {ConnectionId}.", canvasId, connectionId);
            await Clients.Caller.ReceiveError(new ErrorDto("ServerError", "State Error", 500, "Failed to retrieve initial canvas state."));
            Context.Abort(); 
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;

        
        if (Context.Items.TryGetValue(CanvasIdItemKey, out object? canvasIdObj) && canvasIdObj is int canvasId)
        {
            string groupName = CanvasHelpers.GetCanvasGroupName(canvasId);

            await Groups.RemoveFromGroupAsync(connectionId, groupName, Context.ConnectionAborted);
             _logger.LogInformation("Client {ConnectionId} removed from group '{GroupName}'.", connectionId, groupName);
        }
        else
        {
             _logger.LogWarning("Could not determine canvas group for disconnecting client {ConnectionId}.", connectionId);
        }


        if (exception != null)
        {
             _logger.LogWarning(exception, "Client disconnected with error: {ConnectionId}", connectionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", connectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    // PlacePixel method remains largely the same, but notifier call inside it is now redundant
    // if the notifier itself targets the group. Let's keep the notifier call for now,
    // assuming the notifier implementation handles the group targeting.
    public async Task PlacePixel(int canvasId, int x, int y, int r, int g, int b)
    {
        string? userIdString = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId) || userId <= 0)
        {
            _logger.LogError("Unauthorized pixel placement attempt: Invalid User ID claim for Connection {ConnectionId}.", Context.ConnectionId);
            await Clients.Caller.ReceiveError(new ErrorDto("Unauthorized", "Invalid user identifier.", 401, "User ID claim is missing or invalid."));
            Context.Abort();
            return;
        }

         _logger.LogInformation("Received PlacePixel request via Hub from User {UserId} for Canvas {CanvasId} at ({X},{Y})", userId, canvasId, x, y);
        var command = new PlacePixelHubCommand { UserId = userId, CanvasId = canvasId, X = x, Y = y, R = r, G = g, B = b };

        try
        {
            PixelDto placedPixel = await _mediator.Send(command);
             _logger.LogInformation("Pixel placement successful via Hub for User {UserId}. Notifying group.", userId);

            
            await _pixelNotifier.NotifyPixelUpdateAsync(placedPixel);
        }
        catch (DomainValidationException ex)
        {
             _logger.LogWarning(ex, "Domain validation failed during Hub PlacePixel for User {UserId}, Canvas {CanvasId}", userId, canvasId);
             await Clients.Caller.ReceiveError(new ErrorDto(ex.GetType().Name, "Placement rejected.", 400, ex.Message));
        }
        catch (ApplicationValidationException ex)
        {
             _logger.LogWarning(ex, "Application validation failed during Hub PlacePixel for User {UserId}, Canvas {CanvasId}", userId, canvasId);
             await Clients.Caller.ReceiveError(new ErrorDto(ex.GetType().Name, "Invalid input.", 400, ex.Message));
        }
         catch (NotFoundException ex)
        {
             _logger.LogWarning(ex, "Not found during Hub PlacePixel for User {UserId}, Canvas {CanvasId}", userId, canvasId);
             await Clients.Caller.ReceiveError(new ErrorDto(ex.GetType().Name, "Resource not found.", 404, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Hub PlacePixel for User {UserId}, Canvas {CanvasId}", userId, canvasId);
            await Clients.Caller.ReceiveError(new ErrorDto("ServerError", "An unexpected error occurred.", 500, "An internal server error prevented pixel placement."));
        }
    }

    
}
