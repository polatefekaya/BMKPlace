using System;
using BMKPlace.Application.Contracts.DTOs.Canvas;
using Mediator;

namespace BMKPlace.Application.Features.PixelCanvas.Commands.CreateCanvas;

public sealed class CreateCanvasCommand : ICommand<CanvasInfoDto>
{
    public required string Name { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required TimeSpan DefaultCooldown { get; init; }
    public required int ColorPaletteId { get; init; }
    public required int SchoolId { get; init; }
}
