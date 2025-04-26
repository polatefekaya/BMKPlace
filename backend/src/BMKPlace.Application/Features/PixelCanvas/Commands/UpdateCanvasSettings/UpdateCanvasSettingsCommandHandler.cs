using System;
using BMKPlace.Application.Common.Helpers;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Application.Features.PixelCanvas.Commands.CreateCanvas;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.PixelCanvas.Commands.UpdateCanvasSettings;

public class UpdateCanvasSettingsCommandHandler : ICommandHandler<UpdateCanvasSettingsCommand>
{
    private readonly ICanvasRepository _canvasRepository;
    private readonly IColorPaletteRepository _paletteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCanvasSettingsCommandHandler> _logger;

    public UpdateCanvasSettingsCommandHandler(
        ICanvasRepository canvasRepository,
        IColorPaletteRepository paletteRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCanvasSettingsCommandHandler> logger)
    {
        _canvasRepository = canvasRepository;
        _paletteRepository = paletteRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(UpdateCanvasSettingsCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateCanvasSettingsCommand for CanvasId: {CanvasId}", command.CanvasId);

        // --- Get Existing Entity ---
        Domain.Entities.Canvas? canvas = await _canvasRepository.GetByIdAsync(command.CanvasId, cancellationToken);
        if (canvas is null)
        {
            _logger.LogWarning("Canvas not found for UpdateCanvasSettingsCommand. CanvasId: {CanvasId}", command.CanvasId);
            throw new NotFoundException(nameof(Domain.Entities.Canvas), command.CanvasId);
        }
        
        var validationErrors = new Dictionary<string, List<string>>();

        if (!await _paletteRepository.ExistsAsync(command.ColorPaletteId, cancellationToken)) 
        {
            ValidationHelpers.AddValidationError(validationErrors, nameof(command.ColorPaletteId), $"Color Palette with ID {command.ColorPaletteId} not found.");
        }
        // Optional: Check if new name conflicts with another canvas (requires repo method)
        // var existingCanvasWithName = await _canvasRepository.GetByNameAsync(command.Name, cancellationToken);
        // if (existingCanvasWithName != null && existingCanvasWithName.Id != command.CanvasId)
        // {
        //     ValidationHelpers.AddValidationError(validationErrors, nameof(command.Name), $"A canvas with the name '{command.Name}' already exists.");
        // }

        ValidationHelpers.ThrowIfErrorsExist(validationErrors, "Validation failed while updating canvas settings.", _logger, command);

        try
        {
            canvas.UpdateSettings(
                name: command.Name,
                width: command.Width,
                height: command.Height,
                defaultCooldown: command.DefaultCooldown,
                colorPaletteId: command.ColorPaletteId
            );
        }
        catch(DomainValidationException ex)
        {
            throw new ApplicationValidationException(ex.Message, new Dictionary<string, string[]> { { "domain", [ex.Message] } }, ex);
        }


        // --- Persist ---
        // No explicit _canvasRepository.UpdateAsync(canvas) needed if canvas was tracked by context from GetByIdAsync
        // EF Core change tracker handles the update on SaveChangesAsync.
        // Calling Update explicitly is safer if the tracking state is uncertain.
        await _canvasRepository.UpdateAsync(canvas, cancellationToken); // Explicitly mark for update
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated settings for Canvas {CanvasId}", command.CanvasId);

        // Return Unit.Value for command handlers that don't return data
        return Unit.Value;
    }
}
