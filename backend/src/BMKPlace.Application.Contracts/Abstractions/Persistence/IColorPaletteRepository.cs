using System;
using BMKPlace.Domain.Entities;

namespace BMKPlace.Application.Contracts.Abstractions.Persistence;

public interface IColorPaletteRepository
{
    Task<ColorPalette?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(ColorPalette colorPalette, CancellationToken cancellationToken = default);
}
