using System;
using BMKPlace.Domain.Entities;

namespace BMKPlace.Application.Contracts.Abstractions.Persistence;

public interface ICanvasUserContextRepository
{
    Task<CanvasUserContext?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CanvasUserContext?> GetByCanvasAndUserAsync(int canvasId, int userId, CancellationToken cancellationToken = default);
    Task AddAsync(CanvasUserContext context, CancellationToken cancellationToken = default);
    Task UpdateAsync(CanvasUserContext context, CancellationToken cancellationToken = default);
}
