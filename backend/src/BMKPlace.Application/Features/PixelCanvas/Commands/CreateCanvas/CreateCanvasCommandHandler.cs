using System;
using BMKPlace.Application.Common.Helpers;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Application.Contracts.DTOs.Canvas;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace BMKPlace.Application.Features.PixelCanvas.Commands.CreateCanvas;

public sealed class CreateCanvasCommandHandler : ICommandHandler<CreateCanvasCommand, CanvasInfoDto>
{
    private readonly ICanvasRepository _canvasRepository;
    private readonly IColorPaletteRepository _paletteRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCanvasCommandHandler> _logger;

    public CreateCanvasCommandHandler(
        ICanvasRepository canvasRepository,
        IColorPaletteRepository paletteRepository,
        ISchoolRepository schoolRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCanvasCommandHandler> logger)
    {
        _canvasRepository = canvasRepository;
        _paletteRepository = paletteRepository;
        _schoolRepository = schoolRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<CanvasInfoDto> Handle(CreateCanvasCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateCanvasCommand for Name: {CanvasName}, SchoolId: {SchoolId}", command.Name, command.SchoolId);

        var validationErrors = new Dictionary<string, List<string>>();

        if (!await _paletteRepository.ExistsAsync(command.ColorPaletteId, cancellationToken)) // Assumes ExistsAsync added to repo
        {
             ValidationHelpers.AddValidationError(validationErrors, nameof(command.ColorPaletteId), $"Color Palette with ID {command.ColorPaletteId} not found.");
        }
        if (!await _schoolRepository.ExistsAsync(command.SchoolId, cancellationToken))
        {
             ValidationHelpers.AddValidationError(validationErrors, nameof(command.SchoolId), $"School with ID {command.SchoolId} not found.");
        }
        // Optional: Check for unique canvas name (requires new repo method or query)
        // if (await _canvasRepository.ExistsByNameAsync(command.Name, cancellationToken))
        // {
        //     ValidationHelpers.AddValidationError(validationErrors, nameof(command.Name), $"A canvas with the name '{command.Name}' already exists.");
        // }

        ValidationHelpers.ThrowIfErrorsExist(validationErrors, "Validation failed while creating canvas.", _logger, command);

        Domain.Entities.Canvas newCanvas;
        try
        {
             newCanvas = Domain.Entities.Canvas.Create(
                name: command.Name,
                width: command.Width,
                height: command.Height,
                defaultCooldown: command.DefaultCooldown,
                colorPaletteId: command.ColorPaletteId,
                schoolId: command.SchoolId,
                isActive: true 
            );
        }
        catch(DomainValidationException ex)
        {
            throw new ApplicationValidationException(ex.Message, new Dictionary<string, string[]> { { "domain", [ex.Message] } }, ex);
        }


        await _canvasRepository.AddAsync(newCanvas, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

         _logger.LogInformation("Successfully created Canvas with ID {CanvasId}, Name: {CanvasName}, SchoolId: {SchoolId}", newCanvas.Id, newCanvas.Name, newCanvas.SchoolId);

        return new CanvasInfoDto(
            Id: newCanvas.Id,
            Name: newCanvas.Name,
            Width: newCanvas.Width,
            Height: newCanvas.Height
        );
    }
}

// Helper extension method needed for IColorPaletteRepository
public static class ColorPaletteRepositoryExtensions
{
     public static async Task<bool> ExistsAsync(this IColorPaletteRepository repo, int id, CancellationToken cancellationToken = default)
     {
          return await repo.GetByIdAsync(id, cancellationToken) != null;
     }
}
