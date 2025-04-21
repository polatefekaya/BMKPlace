using System;

namespace BMKPlace.Domain.Primitives;

public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(TId id) : base(id) {}

    protected AggregateRoot() : base() {}

    protected void AddDominEvent(IDomainEvent domainEvent){
        ArgumentNullException.ThrowIfNull(domainEvent, nameof(domainEvent));
        _domainEvents.Add(domainEvent);
    }

    protected void ClearDomainEvents(){
        _domainEvents.Clear();
    }
}
