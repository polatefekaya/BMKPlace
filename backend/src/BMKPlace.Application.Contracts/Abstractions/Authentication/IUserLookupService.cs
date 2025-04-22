using System;

namespace BMKPlace.Application.Contracts.Abstractions.Authentication;

public interface IUserLookupService
{
    Task<int?> FindUserIdByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
}
