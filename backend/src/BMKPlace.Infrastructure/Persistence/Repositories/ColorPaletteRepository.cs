using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BMKPlace.Infrastructure.Persistence.Repositories;

internal sealed class ColorPaletteRepository : IColorPaletteRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ColorPaletteRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ColorPalette?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Need to explicitly include the owned collection if we want it loaded.
        // EF Core loads owned types by default when querying the owner,
        // but explicit Include doesn't hurt and makes intent clear.
        // Since `_allowedColors` is mapped via `OwnsMany`, EF Core should handle loading it.
        // Let's rely on default behavior first, but keep Include in mind if needed.
        return await _dbContext.ColorPalettes
                              // .Include("_allowedColors") // Use string name for private field if needed, BUT often not required for OwnsMany
                              .FirstOrDefaultAsync(cp => cp.Id == id, cancellationToken);
        // Using FirstOrDefaultAsync instead of FindAsync because FindAsync doesn't support Include easily.
    }

    public async Task AddAsync(ColorPalette colorPalette, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(colorPalette);
        await _dbContext.ColorPalettes.AddAsync(colorPalette, cancellationToken);
    }
}
