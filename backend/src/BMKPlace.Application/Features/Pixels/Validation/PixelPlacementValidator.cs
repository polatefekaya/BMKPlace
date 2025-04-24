using System;
using BMKPlace.Application.Contracts.Abstractions.Validation;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.Exceptions;
using BMKPlace.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.Pixels.Validation;

public class PixelPlacementValidator : IPixelPlacementValidator
{
    private readonly ILogger<PixelPlacementValidator> _logger;

    public PixelPlacementValidator(ILogger<PixelPlacementValidator> logger)
    {
        _logger = logger;
    }
    
    public Task ValidatePlacementRulesAsync(
        Domain.Entities.Canvas canvas,
        CanvasUserContext userContext,
        ColorPalette palette,
        Coordinate coordinate,
        Color color,
        DateTimeOffset operationTimestamp,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Performing business rule validation for placing pixel at {Coordinate} on Canvas {CanvasId} by User {UserId}",
            coordinate, canvas.Id, userContext.UserId);

        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(userContext);
        ArgumentNullException.ThrowIfNull(palette);
        ArgumentNullException.ThrowIfNull(coordinate);
        ArgumentNullException.ThrowIfNull(color);

        if (!canvas.IsActive)
        {
            throw new DomainValidationException($"Canvas '{canvas.Name}' (ID: {canvas.Id}) is not active.");
        }

        if (userContext.IsBanned)
        {
            _logger.LogWarning("User {UserId} attempted action on Canvas {CanvasId} while banned.", userContext.UserId, canvas.Id);
            throw new UserBannedException(userContext.UserId, canvas.Id);
        }

        if (userContext.IsOnCooldown(operationTimestamp, canvas.DefaultCooldown))
        {
            DateTimeOffset cooldownEndTime = userContext.GetCooldownEndTime(canvas.DefaultCooldown)!.Value;
            _logger.LogInformation("User {UserId} on Canvas {CanvasId} is on cooldown until {CooldownEndTime}.", userContext.UserId, canvas.Id, cooldownEndTime);
            throw new UserCooldownException(userContext.UserId, canvas.Id, cooldownEndTime);
        }

        if (!canvas.IsCoordinateWithinBounds(coordinate))
        {
            _logger.LogWarning("Coordinate {Coordinate} is out of bounds for Canvas {CanvasId} ({Width}x{Height}).", coordinate, canvas.Id, canvas.Width, canvas.Height);
            throw new CoordinatesOutOfBoundsException(coordinate, canvas.Width, canvas.Height);
        }

        if (!palette.IsColorAllowed(color))
        {
            _logger.LogWarning("Color {Color} is not allowed by Palette {PaletteId} on Canvas {CanvasId}.", color, palette.Id, canvas.Id);
            throw new ColorNotAllowedException(color, canvas.Id);
        }

        _logger.LogDebug("Business rule validation passed successfully.");

        return Task.CompletedTask;
    }
}

