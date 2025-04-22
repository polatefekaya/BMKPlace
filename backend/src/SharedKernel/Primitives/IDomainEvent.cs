using System;

namespace SharedKernel.Primitives;

public interface IDomainEvent
{
    public Guid EventId {get;}

    public DateTimeOffset OccuredOnUtc {get;}
}
