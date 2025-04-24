using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BMKPlace.Infrastructure.Persistence.Repositories;

internal sealed class CanvasUserContextRepository : ICanvasUserContextRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CanvasUserContextRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<CanvasUserContext?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CanvasUserContexts.FindAsync([id], cancellationToken: cancellationToken);
    }

    public async Task<CanvasUserContext?> GetByCanvasAndUserAsync(int canvasId, int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CanvasUserContexts
            .FirstOrDefaultAsync(ctx => ctx.CanvasId == canvasId && ctx.UserId == userId, cancellationToken);
            // AsNoTracking() if read-only? Depends if it's used for updates later in the same UoW.
    }

    public async Task AddAsync(CanvasUserContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        await _dbContext.CanvasUserContexts.AddAsync(context, cancellationToken);
    }

    public Task UpdateAsync(CanvasUserContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        _dbContext.CanvasUserContexts.Update(context);
        return Task.CompletedTask;
    }
}
