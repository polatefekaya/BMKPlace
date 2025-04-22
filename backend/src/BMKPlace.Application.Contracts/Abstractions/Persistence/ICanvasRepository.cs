using System;
using BMKPlace.Domain.Entities;

namespace BMKPlace.Application.Contracts.Abstractions.Persistence;

public interface ICanvasRepository
{
    Task<Canvas?> GetByIdAsync(int id, CancellationToken cancellationToken = default); 
    Task AddAsync(Canvas canvas, CancellationToken cancellationToken = default);
    Task UpdateAsync(Canvas canvas, CancellationToken cancellationToken = default);
}
