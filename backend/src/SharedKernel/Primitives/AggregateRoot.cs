using System;

namespace SharedKernel.Primitives;

public abstract class AggregateRoot<TId> : Entity<TId>, IHasDomainEvents where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(TId id) : base(id) {}

    protected AggregateRoot() : base() {}

    protected void AddDomainEvent(IDomainEvent domainEvent){
        ArgumentNullException.ThrowIfNull(domainEvent, nameof(domainEvent));
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents(){
        _domainEvents.Clear();
    }
}
