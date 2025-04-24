using System;

namespace BMKPlace.Application.Contracts.Abstractions.Authentication;

public interface IAllowedDomainChecker
{
    Task<bool> IsDomainAllowedAsync(string email, CancellationToken cancellationToken = default);
}
