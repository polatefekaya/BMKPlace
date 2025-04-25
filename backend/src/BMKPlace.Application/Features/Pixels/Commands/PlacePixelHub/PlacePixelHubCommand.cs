using BMKPlace.Application.Contracts.DTOs.Pixels;
using Mediator;

namespace BMKPlace.Application.Features.Pixels.Commands.PlacePixelHub;

public sealed record PlacePixelHubCommand : ICommand<PixelDto>
{
    public required int UserId { get; init; }
    public required int CanvasId { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }
    public required int R { get; init; }
    public required int G { get; init; }
    public required int B { get; init; }
}
