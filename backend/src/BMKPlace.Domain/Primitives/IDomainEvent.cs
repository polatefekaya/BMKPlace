using System;

namespace BMKPlace.Domain.Primitives;

public interface IDomainEvent
{
    public Guid EventId {get;}

    public DateTimeOffset OccuredOnUtc {get;}
}
