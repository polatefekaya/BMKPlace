using System;
using BMKPlace.Domain.ValueObjects;
using SharedKernel.Exceptions;

namespace BMKPlace.Domain.Exceptions;

public class ColorNotAllowedException : DomainValidationException
{
    public Color AttemptedColor { get; }
    public int CanvasId { get; } // Or ColorPaletteId?

    public ColorNotAllowedException(Color color, int canvasId)
        : base($"Color rgb({color.Red},{color.Green},{color.Blue}) is not allowed on canvas {canvasId}.")
    {
        AttemptedColor = color;
        CanvasId = canvasId;
    }
}
