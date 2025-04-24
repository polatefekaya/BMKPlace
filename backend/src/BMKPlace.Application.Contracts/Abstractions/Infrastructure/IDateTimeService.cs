using System;

namespace BMKPlace.Application.Contracts.Abstractions.Infrastructure;

public interface IDateTimeService
{
    DateTimeOffset UtcNow {get;}
}
