using System;
using SharedKernel.Exceptions;

namespace BMKPlace.Domain.Exceptions;

public class TimestampOutOfOrderException : DomainValidationException
{
    public DateTimeOffset ExistingTimestamp { get; }
    public DateTimeOffset AttemptedTimestamp { get; }

    public TimestampOutOfOrderException(DateTimeOffset existing, DateTimeOffset attempted)
        : base($"Attempted operation timestamp ({attempted}) is not newer than existing timestamp ({existing}).")
    {
        ExistingTimestamp = existing;
        AttemptedTimestamp = attempted;
    }
}
