using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;
using SharedKernel.Primitives;

namespace BMKPlace.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(ApplicationDbContext dbContext, IMediator mediator, ILogger<UnitOfWork> logger)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting SaveChangesAsync with domain event dispatching.");

        List<IDomainEvent> eventsToDispatch = CollectDomainEvents();

        int result = await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("SaveChangesAsync result: {ResultCount} entities saved.", result);

        if (result > 0 && eventsToDispatch.Count > 0)
        {
            _logger.LogInformation("Dispatching {EventCount} domain event(s)...", eventsToDispatch.Count);
            foreach (IDomainEvent domainEvent in eventsToDispatch)
            {
                try
                {
                    _logger.LogDebug("Publishing domain event: {EventType} ({EventId})", domainEvent.GetType().Name, domainEvent.EventId);
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing domain event {EventType} ({EventId}). The core transaction was committed, but this event handler failed.", domainEvent.GetType().Name, domainEvent.EventId);
                }
            }
            _logger.LogInformation("Finished dispatching domain events.");
        } else if (eventsToDispatch.Count > 0)
        {
             _logger.LogWarning("Changes were saved ({ResultCount}), but no domain events were collected to dispatch.", result);
        }

        return result;
    }

    private List<IDomainEvent> CollectDomainEvents()
    {
        List<IHasDomainEvents> aggregateRoots = _dbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .Select(entry => entry.Entity)
            .ToList();

        if(aggregateRoots.Count == 0)
        {
            _logger.LogDebug("No aggregate roots with pending domain events found in ChangeTracker.");
            return []; 
        }

        _logger.LogDebug("Found {AggregateCount} aggregate root(s) with pending domain events.", aggregateRoots.Count);

        List<IDomainEvent> domainEvents = aggregateRoots
            .SelectMany(aggregate =>
            {
                var events = aggregate.DomainEvents.ToList(); // Create copy before clearing
                return events;
            })
            .ToList();

        aggregateRoots.ForEach(aggregate => aggregate.ClearDomainEvents());
        _logger.LogDebug("Collected {EventCount} domain event(s) and cleared them from aggregates.", domainEvents.Count);

        return domainEvents;
    }
}
