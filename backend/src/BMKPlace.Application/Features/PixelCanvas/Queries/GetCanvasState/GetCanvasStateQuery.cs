using BMKPlace.Application.Contracts.DTOs.Canvas;
using Mediator;

namespace BMKPlace.Application.Features.PixelCanvas.Queries.GetCanvasState;

public sealed record class GetCanvasStateQuery : IQuery<CanvasStateDto>
{
    public required int CanvasId {get; init;}
    public bool IgnoreCache {get; init;} = false;
}
