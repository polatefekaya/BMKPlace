using System;
using BMKPlace.Domain.ValueObjects;
using SharedKernel.Exceptions;

namespace BMKPlace.Domain.Exceptions;

public class CoordinatesOutOfBoundsException : DomainValidationException
{
    public Coordinate AttemptedCoordinate { get; }
    public int CanvasWidth { get; }
    public int CanvasHeight { get; }

    public CoordinatesOutOfBoundsException(Coordinate coordinate, int canvasWidth, int canvasHeight)
        : base($"Coordinate ({coordinate.X}, {coordinate.Y}) is outside the canvas bounds [0..{canvasWidth-1}, 0..{canvasHeight-1}].")
    {
        AttemptedCoordinate = coordinate;
        CanvasWidth = canvasWidth;
        CanvasHeight = canvasHeight;
    }
}
