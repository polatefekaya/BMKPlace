using Mediator;

namespace BMKPlace.Application.Features.PixelCanvas.Commands.UpdateCanvasSettings;

public sealed record UpdateCanvasSettingsCommand : ICommand
{
    public required int CanvasId { get; init; }

    public required string Name { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required TimeSpan DefaultCooldown { get; init; }
    public required int ColorPaletteId { get; init; }
}
