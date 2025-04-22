namespace BMKPlace.Application.Contracts.DTOs.Canvas;

public record CanvasStateDto(
    int CanvasId,
    int Width,
    int Height,
    int ColorPaletteId,
    byte[] PixelData,
    DateTimeOffset StateTimestamp
);