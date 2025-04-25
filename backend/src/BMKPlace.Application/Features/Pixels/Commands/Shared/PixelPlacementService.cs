using System;
using BMKPlace.Application.Common.Helpers;
using BMKPlace.Application.Contracts.Abstractions.Application.Pixels;
using BMKPlace.Application.Contracts.Abstractions.Infrastructure;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Application.Contracts.Abstractions.Validation;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.Pixels.Commands.Shared;

public class PixelPlacementService : IPixelPlacementService
{
    private readonly ICanvasRepository _canvasRepository;
    private readonly IColorPaletteRepository _colorPaletteRepository;
    private readonly ICanvasUserContextRepository _canvasUserContextRepository;
    private readonly IPixelRepository _pixelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICanvasCache _canvasCache;
    private readonly IPixelPlacementValidator _placementValidator;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<PixelPlacementService> _logger;

    public PixelPlacementService(
        ICanvasRepository canvasRepository,
        IColorPaletteRepository colorPaletteRepository,
        ICanvasUserContextRepository canvasUserContextRepository,
        IPixelRepository pixelRepository,
        IUnitOfWork unitOfWork,
        ICanvasCache canvasCache,
        IPixelPlacementValidator placementValidator,
        IDateTimeService dateTimeService,
        ILogger<PixelPlacementService> logger)
    {
        _canvasRepository = canvasRepository;
        _colorPaletteRepository = colorPaletteRepository;
        _canvasUserContextRepository = canvasUserContextRepository;
        _pixelRepository = pixelRepository;
        _unitOfWork = unitOfWork;
        _canvasCache = canvasCache;
        _placementValidator = placementValidator;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }
    public async Task<PixelDto> ExecutePlacementAsync(int userId, int canvasId, int x, int y, int r, int g, int b, string commandTypeName, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Executing shared pixel placement logic via {CommandType} for Canvas {CanvasId}, User {UserId} at ({X},{Y})",
            commandTypeName, canvasId, userId, x, y);

        var (coordinate, color) = PerformInputValidationAndCreateVOs(userId, canvasId, x, y, r, g, b);
        var operationTimestamp = _dateTimeService.UtcNow;
        var (canvas, userContext, palette) = await FetchRequiredDomainStateAsync(canvasId, userId, cancellationToken);

        await _placementValidator.ValidatePlacementRulesAsync(canvas, userContext, palette, coordinate, color, operationTimestamp, cancellationToken);

        Pixel pixelEvent = Pixel.Create(
            canvasId: canvasId,
            coordinate: coordinate,
            color: color,
            userId: userId,
            timestamp: operationTimestamp
        );

        await _pixelRepository.AddAsync(pixelEvent, cancellationToken);
        userContext.RecordPixelPlacement(operationTimestamp);

        int changes = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (changes <= 0)
        {
            _logger.LogWarning("SaveChangesAsync reported 0 changes for {CommandType}. User {UserId}, Canvas {CanvasId}", commandTypeName, userId, canvasId);
        }
        if (pixelEvent.Id <= 0)
        {
             _logger.LogError("Critical: Pixel ID was not populated after SaveChangesAsync for {CommandType}. User {UserId}, Canvas {CanvasId}", commandTypeName, userId, canvasId);
             throw new InvalidOperationException("Failed to retrieve generated Pixel ID after saving.");
        }


        PixelDto resultDto = new PixelDto(
            pixelEvent.CanvasId, pixelEvent.Coordinate.X, pixelEvent.Coordinate.Y,
            pixelEvent.Color.Red, pixelEvent.Color.Green, pixelEvent.Color.Blue,
            pixelEvent.UserId, pixelEvent.Timestamp
        );

        // Update cache (awaiting for consistency)
        try
        {
            await _canvasCache.UpdatePixelInCacheAsync(resultDto, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update canvas cache during shared placement logic. User {UserId}, Canvas {CanvasId}", userId, canvasId);
            // Do not fail the operation, just log cache failure.
        }

        _logger.LogInformation("Shared pixel placement logic completed successfully via {CommandType} for Pixel ID {PixelId}.", commandTypeName, pixelEvent.Id);
        return resultDto;
    }

    private (Coordinate Coordinate, Color Color) PerformInputValidationAndCreateVOs(int userId, int canvasId, int x, int y, int r, int g, int b)
    {
         _logger.LogDebug("Performing input validation and creating Value Objects...");
         Dictionary<string, List<string>> validationErrors = [];
        if (canvasId <= 0) ValidationHelpers.AddValidationError(validationErrors, nameof(canvasId), "Canvas ID must be positive.");
        if (userId <= 0) ValidationHelpers.AddValidationError(validationErrors, nameof(userId), "User ID must be valid.");
        // Basic checks done, VOs handle domain validity.
        ValidationHelpers.ThrowIfErrorsExist(validationErrors, "Core validation failed for pixel placement request.", _logger);

        try
        {
            Coordinate coordinate = Coordinate.Create(x, y);
            Color color = Color.Create(r, g, b);
             _logger.LogDebug("Input validation passed. VOs created.");
            return (coordinate, color);
        }
        catch (DomainValidationException ex)
        {
             _logger.LogWarning(ex, "Value Object creation failed during input validation.");
            var errors = new Dictionary<string, string[]> { { "input", new[] { ex.Message } } };
            throw new ApplicationValidationException("Invalid coordinate or color value.", errors, ex);
        }
    }

     private async Task<(Canvas Canvas, CanvasUserContext UserContext, ColorPalette Palette)> FetchRequiredDomainStateAsync(int canvasId, int userId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Fetching required domain state: Canvas {CanvasId}, UserContext for User {UserId}", canvasId, userId);
        Task<Canvas?> canvasTask = _canvasRepository.GetByIdAsync(canvasId, cancellationToken);
        Task<CanvasUserContext?> userContextTask = _canvasUserContextRepository.GetByCanvasAndUserAsync(canvasId, userId, cancellationToken);
        await Task.WhenAll(canvasTask, userContextTask);

        Canvas canvas = await canvasTask ?? throw new NotFoundException(nameof(Canvas), canvasId);
        CanvasUserContext userContext = await userContextTask ?? throw new NotFoundException($"User context not found for User ID {userId} on Canvas ID {canvasId}. User might need to join canvas first.");

        ColorPalette palette = await _colorPaletteRepository.GetByIdAsync(canvas.ColorPaletteId, cancellationToken)
            ?? throw new NotFoundException(nameof(ColorPalette), canvas.ColorPaletteId);

         _logger.LogDebug("Domain state fetched successfully.");
        return (canvas, userContext, palette);
    }
}
