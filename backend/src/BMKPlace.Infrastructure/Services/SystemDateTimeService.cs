using System;
using BMKPlace.Application.Contracts.Abstractions.Infrastructure;

namespace BMKPlace.Infrastructure.Services;

public class SystemDateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
