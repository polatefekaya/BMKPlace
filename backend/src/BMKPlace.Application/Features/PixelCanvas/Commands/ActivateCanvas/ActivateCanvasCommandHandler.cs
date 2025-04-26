using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.PixelCanvas.Commands.ActivateCanvas;

public  sealed class ActivateCanvasCommandHandler : ICommandHandler<ActivateCanvasCommand>
{
    private readonly ICanvasRepository _canvasRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateCanvasCommandHandler> _logger;

    public ActivateCanvasCommandHandler(
        ICanvasRepository canvasRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateCanvasCommandHandler> logger)
    {
        _canvasRepository = canvasRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(ActivateCanvasCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ActivateCanvasCommand for CanvasId: {CanvasId}", command.CanvasId);

        var canvas = await _canvasRepository.GetByIdAsync(command.CanvasId, cancellationToken);
        if (canvas is null)
        {
            _logger.LogWarning("Canvas not found for ActivateCanvasCommand. CanvasId: {CanvasId}", command.CanvasId);
            throw new NotFoundException(nameof(Domain.Entities.Canvas), command.CanvasId);
        }

        // Call domain logic
        canvas.Activate();
        _logger.LogInformation("Canvas {CanvasId} activated in domain model.", command.CanvasId);

        // Persist changes (UpdateAsync marks entity as modified if needed)
        await _canvasRepository.UpdateAsync(canvas, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Canvas {CanvasId} activation persisted.", command.CanvasId);


        return Unit.Value;
    }
}
