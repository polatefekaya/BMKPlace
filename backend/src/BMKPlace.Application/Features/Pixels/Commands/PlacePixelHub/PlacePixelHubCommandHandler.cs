using BMKPlace.Application.Contracts.Abstractions.Application.Pixels;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using Mediator;
using Microsoft.Extensions.Logging; 

namespace BMKPlace.Application.Features.Pixels.Commands.PlacePixelHub;

public sealed class PlacePixelHubCommandHandler : ICommandHandler<PlacePixelHubCommand, PixelDto>
{
    private readonly IPixelPlacementService _placementService; 
    private readonly ILogger<PlacePixelHubCommandHandler> _logger;

    public PlacePixelHubCommandHandler(
        IPixelPlacementService placementService,
        ILogger<PlacePixelHubCommandHandler> logger)
    {
        _placementService = placementService;
        _logger = logger;
    }

    public async ValueTask<PixelDto> Handle(PlacePixelHubCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PlacePixelHubCommand for Canvas {CanvasId}, User {UserId} at ({X},{Y})",
            command.CanvasId, command.UserId, command.X, command.Y);

        PixelDto resultDto = await _placementService.ExecutePlacementAsync(
            command.UserId,
            command.CanvasId,
            command.X, command.Y,
            command.R, command.G, command.B,
            nameof(PlacePixelHubCommand),
            cancellationToken);

        _logger.LogInformation("PlacePixelHubCommand completed.");
        return resultDto;
    }
}