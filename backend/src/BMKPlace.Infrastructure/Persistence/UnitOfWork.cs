using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;

namespace BMKPlace.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Implement Domain Event Dispatching logic here before saving changes.
        // 1. Get all tracked entities implementing AggregateRoot<TId>.
        // 2. Collect all domain events from their DomainEvents collection.
        // 3. Call base.SaveChangesAsync().
        // 4. If successful, publish collected events via Mediator.
        // 5. Clear events from entities.

        // For now, just save changes directly.
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
