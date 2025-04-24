using System;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using Mediator;

namespace BMKPlace.Application.Features.Pixels.Queries.GetLatestPixelForCoordinate;

public sealed record GetLatestPixelForCoordinateQuery :IQuery<PixelDto?>
{
    public required int CanvasId {get; init;}
    public required int X {get; init;}
    public required int Y {get; init;}
}
