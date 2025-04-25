using System;
using BMKPlace.Application.Contracts.Abstractions.Realtime;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BMKPlace.Infrastructure.Realtime;

internal sealed class PixelNotifier : IPixelNotifier
{
    private readonly IHubContext<PixelHub, IPixelHubClient> _hubContext;
    private readonly ILogger<PixelNotifier> _logger;

    public PixelNotifier(
        IHubContext<PixelHub, IPixelHubClient> hubContext,
        ILogger<PixelNotifier> logger)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task NotifyPixelUpdateAsync(PixelDto pixelDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pixelDto);
        _logger.LogInformation("Notifying clients of pixel update via PixelNotifier for Canvas {CanvasId} at ({X},{Y})",
            pixelDto.CanvasId, pixelDto.X, pixelDto.Y);
        try
        {
            // Send to ALL clients using the strongly-typed client interface method
            await _hubContext.Clients.All.ReceivePixelUpdate(pixelDto);
            _logger.LogDebug("Successfully sent pixel update notification via PixelNotifier.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send pixel update notification via PixelNotifier.");
            // Decide whether to re-throw or just log
        }
    }
}
