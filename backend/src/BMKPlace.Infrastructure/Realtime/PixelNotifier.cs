using System;
using BMKPlace.Application.Contracts.Abstractions.Realtime;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SharedKernel.Helpers.Canvas;

namespace BMKPlace.Infrastructure.Realtime;

internal sealed class PixelNotifier : IPixelNotifier
{
    private readonly IPixelBroadcastService _broadcastService;
    private readonly ILogger<PixelNotifier> _logger;

    public PixelNotifier(
        IPixelBroadcastService broadcastService,
        ILogger<PixelNotifier> logger)
    {
        _broadcastService = broadcastService;
        _logger = logger;
    }

    public async Task NotifyPixelUpdateAsync(PixelDto pixelDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pixelDto);

        _logger.LogInformation("Triggering broadcast for pixel update via IPixelBroadcastService for Canvas {CanvasId} at ({X},{Y})",
            pixelDto.CanvasId, pixelDto.X, pixelDto.Y);
        try
        {
            // Delegate the actual broadcasting to the implementation provided by the outer layer (API)
            await _broadcastService.BroadcastPixelUpdateAsync(pixelDto, cancellationToken);

            _logger.LogDebug("Successfully delegated pixel update broadcast.");
        }
        catch (Exception ex)
        {
            // Log error during delegation
            _logger.LogError(ex, "Failed to trigger pixel update broadcast via IPixelBroadcastService.");
            // Decide whether to re-throw or just log
        }
    }
}
