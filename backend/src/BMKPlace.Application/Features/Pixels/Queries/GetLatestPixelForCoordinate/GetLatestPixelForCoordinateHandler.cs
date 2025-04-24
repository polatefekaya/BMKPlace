using System;
using BMKPlace.Application.Common.Helpers;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Application.Contracts.DTOs.Pixels;
using BMKPlace.Domain.Entities;
using BMKPlace.Domain.ValueObjects;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.Pixels.Queries.GetLatestPixelForCoordinate;

public class GetLatestPixelForCoordinateHandler : IQueryHandler<GetLatestPixelForCoordinateQuery, PixelDto?>
{
    private readonly IPixelRepository _pixelRepository;
    private readonly ILogger<GetLatestPixelForCoordinateHandler> _logger;

    public GetLatestPixelForCoordinateHandler(IPixelRepository pixelRepository, ILogger<GetLatestPixelForCoordinateHandler> logger)
    {
        _pixelRepository = pixelRepository ?? throw new ArgumentNullException(nameof(pixelRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<PixelDto?> Handle(GetLatestPixelForCoordinateQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetLatestPixelForCoordinateQuery for CanvasId {CanvasId} at ({X},{Y})",
            query.CanvasId, query.X, query.Y);

        Coordinate coordinate;
        try
        {
            ValidateQueryInput(query); 
            coordinate = Coordinate.Create(query.X, query.Y); 
        }
        catch (ApplicationValidationException ex)
        {
            _logger.LogWarning(ex, "Input validation failed for GetLatestPixelForCoordinateQuery.");
            throw;
        }
        catch (DomainValidationException ex) 
        {
            _logger.LogWarning(ex, "Invalid coordinate value provided.");
            
            var errors = new Dictionary<string, string[]> { { "coordinate", new[] { ex.Message } } };
            throw new ApplicationValidationException("Invalid coordinate value provided.", errors, ex);
        }

        _logger.LogDebug("Fetching latest pixel for CanvasId {CanvasId}, Coordinate {Coordinate}", query.CanvasId, coordinate);
        Pixel? latestPixel = await _pixelRepository.GetLatestPixelForCoordinateAsync(query.CanvasId, coordinate, cancellationToken);

        if (latestPixel is null)
        {
            _logger.LogInformation("No pixel found at Coordinate {Coordinate} for CanvasId {CanvasId}.", coordinate, query.CanvasId);
            return null; 
        }

        _logger.LogDebug("Mapping found Pixel {PixelId} to PixelDto.", latestPixel.Id);
        PixelDto dto = new(
            latestPixel.CanvasId,
            latestPixel.Coordinate.X,
            latestPixel.Coordinate.Y,
            latestPixel.Color.Red,
            latestPixel.Color.Green,
            latestPixel.Color.Blue,
            latestPixel.UserId,
            latestPixel.Timestamp
        );

        _logger.LogInformation("GetLatestPixelForCoordinateQuery handled successfully for CanvasId {CanvasId} at {Coordinate}.", query.CanvasId, coordinate);
        return dto;
    }

    private void ValidateQueryInput(GetLatestPixelForCoordinateQuery query)
    {
        _logger.LogDebug("Validating query input...");
        Dictionary<string, List<string>> validationErrors = new();

        if (query.CanvasId <= 0)
            ValidationHelpers.AddValidationError(validationErrors, nameof(query.CanvasId), "Canvas ID must be positive.");

        ValidationHelpers.ThrowIfErrorsExist(validationErrors, "Validation failed for get pixel request.", _logger, query);
        _logger.LogDebug("Input validation passed.");
    }
}
