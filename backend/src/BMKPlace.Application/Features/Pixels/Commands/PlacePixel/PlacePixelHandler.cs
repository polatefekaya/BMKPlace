using System;
using BMKPlace.Application.Contracts.Abstractions.Infrastructure;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Application.Contracts.Abstractions.Realtime;
using BMKPlace.Application.Contracts.Abstractions.Validation;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.ValueObjects;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.Pixels.Commands.PlacePixel;

public class PlacePixelHandler : ICommandHandler<PlacePixelCommand, PixelDto>
{
    private readonly ICanvasRepository _canvasRepository;
    private readonly IColorPaletteRepository _colorPaletteRepository;
    private readonly ICanvasUserContextRepository _canvasUserContextRepository;
    private readonly IPixelRepository _pixelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICanvasCache _canvasCache;
    private readonly IPixelNotifier _pixelNotifier;
    private readonly IPixelPlacementValidator _placementValidator;
    private readonly ILogger<PlacePixelHandler> _logger;
    public PlacePixelHandler(
        ICanvasRepository canvasRepository,
        IColorPaletteRepository colorPaletteRepository,
        ICanvasUserContextRepository canvasUserContextRepository,
        IPixelRepository pixelRepository,
        IUnitOfWork unitOfWork,
        ICanvasCache canvasCache,
        IPixelNotifier pixelNotifier,
        IPixelPlacementValidator placementValidator,
        ILogger<PlacePixelHandler> logger
        )
    {
        _canvasRepository = canvasRepository;
        _colorPaletteRepository = colorPaletteRepository;
        _canvasUserContextRepository = canvasUserContextRepository;
        _pixelRepository = pixelRepository;
        _unitOfWork = unitOfWork;
        _canvasCache = canvasCache;
        _pixelNotifier = pixelNotifier;
        _placementValidator = placementValidator;
        _logger = logger;
    }
    public async ValueTask<PixelDto> Handle(PlacePixelCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PlacePixelCommand for Canvas {CanvasId}, User {UserId} at ({X},{Y})",
            command.CanvasId, command.UserId, command.X, command.Y);

      
        ValidateCommandInput(command); 

        DateTimeOffset operationTimestamp = DateTimeOffset.UtcNow; 

        Coordinate coordinate = Coordinate.Create(command.X, command.Y);
        Color color = Color.Create(command.R, command.G, command.B);

        _logger.LogDebug("Fetching required entities for validation...");
        Task<Canvas?> canvasTask = _canvasRepository.GetByIdAsync(command.CanvasId, cancellationToken);
        Task<CanvasUserContext?> userContextTask = _canvasUserContextRepository.GetByCanvasAndUserAsync(command.CanvasId, command.UserId, cancellationToken);

        await Task.WhenAll(canvasTask, userContextTask);
        Canvas canvas = await canvasTask ?? throw new NotFoundException(nameof(Canvas), command.CanvasId);
        CanvasUserContext userContext = await userContextTask ?? throw new NotFoundException($"User context not found for User ID {command.UserId} on Canvas ID {command.CanvasId}.");

        ColorPalette palette = await _colorPaletteRepository.GetByIdAsync(canvas.ColorPaletteId, cancellationToken)
            ?? throw new NotFoundException(nameof(ColorPalette), canvas.ColorPaletteId);
        _logger.LogDebug("Entities fetched successfully.");


        _logger.LogDebug("Delegating business rule validation to IPixelPlacementValidator.");
        await _placementValidator.ValidatePlacementRulesAsync(
            canvas, userContext, palette, coordinate, color, operationTimestamp, cancellationToken);

        _logger.LogDebug("Business rule validation passed.");

        Pixel pixelEvent = Pixel.Create(
            canvasId: command.CanvasId,
            coordinate: coordinate,
            color: color,
            userId: command.UserId,
            timestamp: operationTimestamp
        );
        _logger.LogDebug("Pixel domain entity created.");

        _logger.LogDebug("Adding Pixel event and updating UserContext last placement time.");
        await _pixelRepository.AddAsync(pixelEvent, cancellationToken);
        userContext.RecordPixelPlacement(operationTimestamp);

        _logger.LogInformation("Saving changes to database via Unit of Work...");
        int changes = await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Database changes saved ({ChangeCount}). Pixel ID {PixelId} generated.", changes, pixelEvent.Id);


        // 7. --- Map Result ---
        // Side effects (cache update, SignalR notification) are now handled by domain event handlers (e.g., PixelPlacedEventHandler)
        // which will be triggered by the mediator *after* this handler completes successfully (post-commit).
        // Therefore, the command handler's responsibility ends here by returning the result.
        PixelDto resultDto = new(
            pixelEvent.CanvasId, pixelEvent.Coordinate.X, pixelEvent.Coordinate.Y,
            pixelEvent.Color.Red, pixelEvent.Color.Green, pixelEvent.Color.Blue,
            pixelEvent.UserId, pixelEvent.Timestamp
            // Add pixelEvent.Id if needed in DTO
        );

        _logger.LogInformation("PlacePixelCommand handled successfully for Pixel ID {PixelId}.", pixelEvent.Id);
        return resultDto;
    }

    private void ValidateCommandInput(PlacePixelCommand command)
    {
        Dictionary<string, string[]> validationErrors = new();
        if (command.CanvasId <= 0) AddError(validationErrors, nameof(command.CanvasId), "Canvas ID must be positive.");
        if (command.UserId <= 0) AddError(validationErrors, nameof(command.UserId), "User ID must be positive.");
        // Coordinate/Color VOs handle their own intrinsic validation (non-negative, 0-255 range)

        if (validationErrors.Count > 0)
        {
            _logger.LogWarning("Input validation failed for PlacePixelCommand: {@ValidationErrors}", validationErrors);
            // Using structured logging for the dictionary
            throw new ApplicationValidationException("Validation failed for pixel placement request.", validationErrors);
        }
    }

    // Helper to build validation error dictionary (same as before)
    private void AddError(Dictionary<string, string[]> errors, string propertyName, string errorMessage)
    {
        // Implementation omitted for brevity... see previous response
        propertyName = propertyName[0].ToString().ToLowerInvariant() + propertyName.Substring(1); // camelCase
        if (!errors.TryAdd(propertyName, new[] { errorMessage })) { errors[propertyName] = errors[propertyName].Append(errorMessage).ToArray(); }
    }
}
