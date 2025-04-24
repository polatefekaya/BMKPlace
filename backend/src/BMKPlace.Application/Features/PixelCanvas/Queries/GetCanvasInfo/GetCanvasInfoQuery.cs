using BMKPlace.Application.Contracts.DTOs.Canvas;
using Mediator;

namespace BMKPlace.Application.Features.PixelCanvas.Queries.GetCanvasInfo;

public sealed record class GetCanvasInfoQuery : IQuery<CanvasInfoDto>
{
    public required int CanvasId {get; init;}
}
