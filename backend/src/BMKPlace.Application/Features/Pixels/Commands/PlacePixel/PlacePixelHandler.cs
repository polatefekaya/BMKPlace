using System;
using BMKPlace.Application.Common.Helpers;
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
    private readonly IDateTimeService _dateTimeService;
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
        IDateTimeService dateTimeService,
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
        _dateTimeService = dateTimeService;
        _logger = logger;
    }
    public async ValueTask<PixelDto> Handle(PlacePixelCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting PlacePixelCommand execution for Canvas {CanvasId}, User {UserId} at ({X},{Y})",
            command.CanvasId, command.UserId, command.X, command.Y);

        var (coordinate, color) = PerformInputValidationAndCreateVOs(command);
        var operationTimestamp = GetOperationTimestamp(); // Use IDateTimeService ideally
        var (canvas, userContext, palette) = await FetchRequiredDomainStateAsync(command.CanvasId, command.UserId, cancellationToken);
        await PerformBusinessRuleValidationAsync(canvas, userContext, palette, coordinate, color, operationTimestamp, cancellationToken);
        var persistedPixel = await ExecuteDomainLogicAndPersistAsync(command, coordinate, color, userContext, operationTimestamp, cancellationToken);

        var resultDto = MapResultToDto(persistedPixel);

        _logger.LogInformation("PlacePixelCommand execution completed successfully for Pixel ID {PixelId}.", persistedPixel.Id);

        return resultDto;
    }

private (Coordinate Coordinate, Color Color) PerformInputValidationAndCreateVOs(PlacePixelCommand command)
    {
        _logger.LogDebug("Performing input validation and creating Value Objects...");

        Dictionary<string, List<string>> validationErrors = new();
        if (command.CanvasId <= 0) ValidationHelpers.AddValidationError(validationErrors, nameof(command.CanvasId), "Canvas ID must be positive.");
        if (command.UserId <= 0) ValidationHelpers.AddValidationError(validationErrors, nameof(command.UserId), "User ID must be positive.");

        ValidationHelpers.ThrowIfErrorsExist(validationErrors, "Validation failed for pixel placement request.", _logger, command);

        try
        {
            Coordinate coordinate = Coordinate.Create(command.X, command.Y);
            Color color = Color.Create(command.R, command.G, command.B);
            _logger.LogDebug("Input validation passed. VOs created.");
            return (coordinate, color);
        }
        catch (DomainValidationException ex) 
        {
            _logger.LogWarning(ex, "Value Object creation failed during input validation.");

            var errors = new Dictionary<string, string[]> { { "colorOrCoordinate", new[] { ex.Message } } };
            throw new ApplicationValidationException("Invalid coordinate or color value.", errors, ex);
        }
    }

    private DateTimeOffset GetOperationTimestamp()
    {
        return _dateTimeService.UtcNow;
    }

    private async Task<(Canvas Canvas, CanvasUserContext UserContext, ColorPalette Palette)> FetchRequiredDomainStateAsync(int canvasId, int userId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Fetching required domain state: Canvas {CanvasId}, UserContext for User {UserId}", canvasId, userId);
        
        Task<Canvas?> canvasTask = _canvasRepository.GetByIdAsync(canvasId, cancellationToken);
        Task<CanvasUserContext?> userContextTask = _canvasUserContextRepository.GetByCanvasAndUserAsync(canvasId, userId, cancellationToken);

        await Task.WhenAll(canvasTask, userContextTask);

        Canvas canvas = await canvasTask ?? throw new NotFoundException(nameof(Canvas), canvasId);
        CanvasUserContext userContext = await userContextTask ?? throw new NotFoundException($"User context not found for User ID {userId} on Canvas ID {canvasId}.");

        ColorPalette palette = await _colorPaletteRepository.GetByIdAsync(canvas.ColorPaletteId, cancellationToken)
            ?? throw new NotFoundException(nameof(ColorPalette), canvas.ColorPaletteId);

        _logger.LogDebug("Domain state fetched successfully.");
        return (canvas, userContext, palette);
    }

    private async Task PerformBusinessRuleValidationAsync(Canvas canvas, CanvasUserContext userContext, ColorPalette palette, Coordinate coordinate, Color color, DateTimeOffset timestamp, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Delegating business rule validation...");

        await _placementValidator.ValidatePlacementRulesAsync(
            canvas, userContext, palette, coordinate, color, timestamp, cancellationToken);
        _logger.LogDebug("Business rule validation successful.");
    }

    private async Task<Pixel> ExecuteDomainLogicAndPersistAsync(PlacePixelCommand command, Coordinate coordinate, Color color, CanvasUserContext userContext, DateTimeOffset timestamp, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing domain logic and persisting changes...");

        Pixel pixelEvent = Pixel.Create(
            canvasId: command.CanvasId,
            coordinate: coordinate,
            color: color,
            userId: command.UserId,
            timestamp: timestamp
        );

        await _pixelRepository.AddAsync(pixelEvent, cancellationToken);
        userContext.RecordPixelPlacement(timestamp);

        _logger.LogInformation("Committing transaction via Unit of Work...");
        int changes = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (pixelEvent.Id <= 0)
        {
            _logger.LogError("Critical: Pixel ID was not populated after SaveChangesAsync. Check DB identity/sequence setup and EF Core configuration.");
            throw new InvalidOperationException("Failed to retrieve generated Pixel ID after saving.");
        }
        _logger.LogInformation("Persistence successful. Pixel ID {PixelId} obtained.", pixelEvent.Id);

        return pixelEvent;
    }

    private PixelDto MapResultToDto(Pixel pixelEvent)
    {
        _logger.LogDebug("Mapping persisted Pixel entity to PixelDto.");
        return new PixelDto(
            pixelEvent.CanvasId, pixelEvent.Coordinate.X, pixelEvent.Coordinate.Y,
            pixelEvent.Color.Red, pixelEvent.Color.Green, pixelEvent.Color.Blue,
            pixelEvent.UserId, pixelEvent.Timestamp
        );
    }
}
