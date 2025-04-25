using BMKPlace.Application.Contracts.Abstractions.Realtime;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using BMKPlace.Application.Features.Pixels.Commands.Shared;
using Mediator;
using Microsoft.Extensions.Logging;

namespace BMKPlace.Application.Features.Pixels.Commands.PlacePixel;

public sealed class PlacePixelHandler : ICommandHandler<PlacePixelCommand, PixelDto>
{
    private readonly PixelPlacementService _placementService;
    private readonly IPixelNotifier _pixelNotifier;
    private readonly ILogger<PlacePixelHandler> _logger;

    public PlacePixelHandler(
        PixelPlacementService placementService,
        IPixelNotifier pixelNotifier, 
        ILogger<PlacePixelHandler> logger)
    {
        _placementService = placementService;
        _pixelNotifier = pixelNotifier;
        _logger = logger;
    }

    public async ValueTask<PixelDto> Handle(PlacePixelCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PlacePixelCommand for Canvas {CanvasId}, User {UserId} at ({X},{Y})",
            command.CanvasId, command.UserId, command.X, command.Y);

        PixelDto resultDto = await _placementService.ExecutePlacementAsync(
            command.UserId,
            command.CanvasId,
            command.X, command.Y,
            command.R, command.G, command.B,
            nameof(PlacePixelCommand),
            cancellationToken);

        try
        {
            await _pixelNotifier.NotifyPixelUpdateAsync(resultDto, cancellationToken);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Failed to notify clients after successful PlacePixelCommand. Pixel ID: {PixelId}", "N/A");
        }

        _logger.LogInformation("PlacePixelCommand completed.");
        return resultDto;
    }
}