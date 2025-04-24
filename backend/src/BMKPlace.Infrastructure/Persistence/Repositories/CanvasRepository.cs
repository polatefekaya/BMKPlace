using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Domain.Entities;

namespace BMKPlace.Infrastructure.Persistence.Repositories;

internal sealed class CanvasRepository : ICanvasRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CanvasRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Canvas?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Canvases.FindAsync([id], cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Canvas canvas, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        await _dbContext.Canvases.AddAsync(canvas, cancellationToken);
    }

    public Task UpdateAsync(Canvas canvas, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        _dbContext.Canvases.Update(canvas);
        return Task.CompletedTask;
    }
}
