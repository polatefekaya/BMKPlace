using System;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.ValueObjects;

namespace BMKPlace.Application.Contracts.Abstractions.Validation;

public interface IPixelPlacementValidator
{
    Task ValidatePlacementRulesAsync(
        Canvas canvas,
        CanvasUserContext userContext,
        ColorPalette palette,
        Coordinate coordinate,
        Color color,
        DateTimeOffset operationTimestamp,
        CancellationToken cancellationToken = default);
}
