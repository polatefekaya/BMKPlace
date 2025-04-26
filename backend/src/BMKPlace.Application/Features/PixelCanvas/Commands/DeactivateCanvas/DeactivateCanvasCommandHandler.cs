using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.PixelCanvas.Commands.DeactivateCanvas;

public sealed class DeactivateCanvasCommandHandler : ICommandHandler<DeactivateCanvasCommand>
{
    private readonly ICanvasRepository _canvasRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateCanvasCommandHandler> _logger;

    public DeactivateCanvasCommandHandler(
        ICanvasRepository canvasRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateCanvasCommandHandler> logger)
    {
        _canvasRepository = canvasRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(DeactivateCanvasCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeactivateCanvasCommand for CanvasId: {CanvasId}", command.CanvasId);

        var canvas = await _canvasRepository.GetByIdAsync(command.CanvasId, cancellationToken);
        if (canvas is null)
        {
            _logger.LogWarning("Canvas not found for DeactivateCanvasCommand. CanvasId: {CanvasId}", command.CanvasId);
            throw new NotFoundException(nameof(Domain.Entities.Canvas), command.CanvasId);
        }

        // Call domain logic
        canvas.Deactivate();
        _logger.LogInformation("Canvas {CanvasId} deactivated in domain model.", command.CanvasId);


        // Persist changes
        await _canvasRepository.UpdateAsync(canvas, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Canvas {CanvasId} deactivation persisted.", command.CanvasId);

        return Unit.Value;
    }
}
