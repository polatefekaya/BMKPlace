using System;
using BMKPlace.Api.Hubs;
using BMKPlace.Application.Contracts.Abstractions.Realtime;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using Microsoft.AspNetCore.SignalR;
using SharedKernel.Helpers.Canvas;

namespace BMKPlace.Api.Services;

public sealed class ApiPixelBroadcastService : IPixelBroadcastService
{
    private readonly IHubContext<PixelHub, IPixelHubClient> _hubContext;
    private readonly ILogger<ApiPixelBroadcastService> _logger;

    public ApiPixelBroadcastService(
        IHubContext<PixelHub, IPixelHubClient> hubContext,
        ILogger<ApiPixelBroadcastService> logger)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task BroadcastPixelUpdateAsync(PixelDto pixelDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pixelDto);
        string groupName = CanvasHelpers.GetCanvasGroupName(pixelDto.CanvasId);

        _logger.LogInformation("Broadcasting pixel update via SignalR to group '{GroupName}' for Canvas {CanvasId} at ({X},{Y})",
            groupName, pixelDto.CanvasId, pixelDto.X, pixelDto.Y);
        try
        {
            await _hubContext.Clients.Group(groupName).ReceivePixelUpdate(pixelDto);

            _logger.LogDebug("Successfully broadcast pixel update via SignalR to group '{GroupName}'.", groupName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast pixel update via SignalR to group '{GroupName}'.", groupName);
            // Re-throwing might be appropriate here if the caller (PixelNotifier) needs to know
            // throw;
        }
    }
}
