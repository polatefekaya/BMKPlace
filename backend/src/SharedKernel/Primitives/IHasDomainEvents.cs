using System;

namespace SharedKernel.Primitives;

public interface IHasDomainEvents
{
    public IReadOnlyCollection<IDomainEvent> DomainEvents {get;}
    public void ClearDomainEvents();
}
