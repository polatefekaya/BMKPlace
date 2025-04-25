using System;
using System.Security.Claims;
using BMKPlace.Application.Contracts.Abstractions.Realtime;
using BMKPlace.Application.Contracts.DTOs.Common;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using BMKPlace.Application.Features.PixelCanvas.Queries.GetCanvasState;
using BMKPlace.Application.Features.Pixels.Commands.PlacePixelHub;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using SharedKernel.Exceptions;

namespace BMKPlace.Api.Hubs;

[Authorize]
public class PixelHub : Hub<IPixelHubClient>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PixelHub> _logger;
    private readonly IPixelNotifier _pixelNotifier;

    public PixelHub(IMediator mediator, ILogger<PixelHub> logger, IPixelNotifier pixelNotifier)
    {
        _mediator = mediator;
        _logger = logger;
        _pixelNotifier = pixelNotifier;
    }

    public async Task PlacePixel(int canvasId, int x, int y, int r, int g, int b)
    {
        // 1. Get User ID from Claims
        string? userIdString = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId) || userId <= 0)
        {
            _logger.LogError("Unauthorized pixel placement attempt: Could not parse User ID from token for Connection {ConnectionId}.", Context.ConnectionId);
            // Send error back to the caller
            await Clients.Caller.ReceiveError(new ErrorDto("Unauthorized", "Invalid user identifier.", 401, "User ID claim is missing or invalid."));
            Context.Abort(); // Disconnect invalid client? Or just return?
            return;
        }

         _logger.LogInformation("Received PlacePixel request via Hub from User {UserId} for Canvas {CanvasId} at ({X},{Y})", userId, canvasId, x, y);


        // 2. Create and Send Command
        var command = new PlacePixelHubCommand
        {
            UserId = userId,
            CanvasId = canvasId,
            X = x,
            Y = y,
            R = r,
            G = g,
            B = b
        };

        try
        {
            PixelDto placedPixel = await _mediator.Send(command);

             _logger.LogInformation("Pixel placement successful via Hub for User {UserId}, Pixel ID {PixelId}. Notifying clients.", userId, "N/A"); // ID not directly available here

            // 3. Notify ALL clients about the update (using the dedicated notifier service)
            // We pass the DTO returned by the handler.
            await _pixelNotifier.NotifyPixelUpdateAsync(placedPixel);

            // Optional: Send confirmation back to the caller?
            // await Clients.Caller.ReceivePlacementConfirmation(placedPixel.Timestamp); // Example
        }
        // 4. Handle Specific Expected Exceptions
        catch (DomainValidationException ex) // Includes Cooldown, Banned, OutOfBounds, ColorNotAllowed etc.
        {
             _logger.LogWarning(ex, "Domain validation failed during Hub PlacePixel for User {UserId}, Canvas {CanvasId}", userId, canvasId);
             // Send specific error details back to the caller
             await Clients.Caller.ReceiveError(new ErrorDto(ex.GetType().Name, "Placement rejected.", 400, ex.Message));
        }
        catch (ApplicationValidationException ex) // Input validation errors from handler
        {
             _logger.LogWarning(ex, "Application validation failed during Hub PlacePixel for User {UserId}, Canvas {CanvasId}", userId, canvasId);
             // Send potentially structured errors back if needed, otherwise just the message
             // string detail = ex.Errors.Count > 0 ? JsonSerializer.Serialize(ex.Errors) : ex.Message; // Example detail
             await Clients.Caller.ReceiveError(new ErrorDto(ex.GetType().Name, "Invalid input.", 400, ex.Message));
        }
         catch (NotFoundException ex)
        {
             _logger.LogWarning(ex, "Not found during Hub PlacePixel for User {UserId}, Canvas {CanvasId}", userId, canvasId);
             await Clients.Caller.ReceiveError(new ErrorDto(ex.GetType().Name, "Resource not found.", 404, ex.Message));
        }
        catch (Exception ex) // Catch-all for unexpected errors
        {
            _logger.LogError(ex, "Unexpected error during Hub PlacePixel for User {UserId}, Canvas {CanvasId}", userId, canvasId);
            // Send generic error back to the caller
            await Clients.Caller.ReceiveError(new ErrorDto("ServerError", "An unexpected error occurred.", 500, "An internal server error prevented pixel placement."));
            // Potentially re-throw if you have higher-level error handling
        }
    }


    public async override Task OnConnectedAsync()
    {
        // Base implementation should be called
        await base.OnConnectedAsync();

        string connectionId = Context.ConnectionId;
        string? userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous"; // Get authenticated user ID if available
        //var transport = Context.Features.Get<IHttpConnectionFeature>()?.TransportType; // Get transport type (WebSockets, etc.)

        _logger.LogInformation("Client connected: {ConnectionId}, User: {UserId}",
            connectionId, userId);

        // Attempt to get canvasId from query string
        HttpContext? httpContext = Context.GetHttpContext();
        string? canvasIdString = httpContext?.Request.Query["canvasId"];

        if (string.IsNullOrWhiteSpace(canvasIdString) || !int.TryParse(canvasIdString, out int canvasId) || canvasId <= 0)
        {
            _logger.LogWarning("Client {ConnectionId} connected without a valid 'canvasId' query string parameter. Cannot send initial state.", connectionId);
            // Optional: Send an error message back to the caller
            // await Clients.Caller.ReceiveError(new ErrorDto("BadRequest", "Missing CanvasId", 400, "A valid 'canvasId' query string parameter is required to connect."));
            // Optional: Abort connection if canvasId is mandatory
            // Context.Abort();
            return;
        }

        _logger.LogInformation("Client {ConnectionId} requested initial state for Canvas {CanvasId}.", connectionId, canvasId);

        // Fetch the initial canvas state using Mediator
        try
        {
            var query = new GetCanvasStateQuery { CanvasId = canvasId, IgnoreCache = false }; // Use cache if available
            var canvasState = await _mediator.Send(query);

            if (canvasState != null)
            {
                 _logger.LogDebug("Sending initial canvas state for Canvas {CanvasId} to Client {ConnectionId}.", canvasId, connectionId);
                // Send the state ONLY to the connecting client
                await Clients.Caller.ReceiveInitialCanvasState(canvasState);
                 _logger.LogInformation("Successfully sent initial state for Canvas {CanvasId} to Client {ConnectionId}.", canvasId, connectionId);
            }
            else
            {
                 // This shouldn't happen if canvas exists, GetCanvasStateQuery handles NotFound
                 _logger.LogWarning("GetCanvasStateQuery returned null for Canvas {CanvasId}. No initial state sent to Client {ConnectionId}.", canvasId, connectionId);
                 // Optionally send an error to the client
                 await Clients.Caller.ReceiveError(new ErrorDto("NotFound", "Canvas State Error", 404, $"Could not retrieve state for Canvas {canvasId}."));
            }
        }
        catch (NotFoundException ex) // Catch specific exceptions if needed
        {
            _logger.LogWarning(ex, "Canvas {CanvasId} not found when fetching initial state for Client {ConnectionId}.", canvasId, connectionId);
            await Clients.Caller.ReceiveError(new ErrorDto("NotFound", "Canvas Not Found", 404, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching or sending initial state for Canvas {CanvasId} to Client {ConnectionId}.", canvasId, connectionId);
            // Optionally send a generic error to the client
            await Clients.Caller.ReceiveError(new ErrorDto("ServerError", "State Error", 500, "Failed to retrieve initial canvas state."));
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
             _logger.LogWarning(exception, "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        }
        return base.OnDisconnectedAsync(exception);
    }
}
