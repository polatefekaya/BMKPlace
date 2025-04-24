using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Application.Contracts.DTOs.Canvas;
using BMKPlace.Domain.Entities;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.PixelCanvas.Queries.GetCanvasInfo;

public class GetCanvasInfoHandler : IQueryHandler<GetCanvasInfoQuery, CanvasInfoDto>
{
    private readonly ICanvasRepository _canvasRepository;
    private readonly ILogger<GetCanvasInfoHandler> _logger;

    public GetCanvasInfoHandler(ICanvasRepository canvasRepository, ILogger<GetCanvasInfoHandler> logger)
    {
        _canvasRepository = canvasRepository;
        _logger = logger;
    }
    public async ValueTask<CanvasInfoDto> Handle(GetCanvasInfoQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCanvasInfoQuery for CanvasId {CanvasId}", query.CanvasId);

        if (query.CanvasId <= 0)
        {
             _logger.LogWarning("Invalid CanvasId {CanvasId} received in GetCanvasInfoQuery.", query.CanvasId);
            throw new ArgumentException("Canvas ID must be positive.", nameof(query.CanvasId));
            // Or: throw new ApplicationValidationException("Invalid input", new Dictionary<string, string[]> {{"canvasId", new[]{"Canvas ID must be positive."}}});
        }

        _logger.LogDebug("Fetching canvas with ID {CanvasId}", query.CanvasId);
        Canvas? canvas = await _canvasRepository.GetByIdAsync(query.CanvasId, cancellationToken);

        if (canvas is null)
        {
            _logger.LogWarning("Canvas {CanvasId} not found during GetCanvasInfoQuery handling.", query.CanvasId);
            throw new NotFoundException(nameof(Canvas), query.CanvasId);
        }

        _logger.LogDebug("Mapping Canvas entity to CanvasInfoDto.");
        CanvasInfoDto dto = new CanvasInfoDto(
            Id: canvas.Id,
            Name: canvas.Name,
            Width: canvas.Width,
            Height: canvas.Height
        );

        _logger.LogInformation("GetCanvasInfoQuery handled successfully for CanvasId {CanvasId}", query.CanvasId);
        return dto;
    }
}
