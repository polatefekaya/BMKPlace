using System;

namespace BMKPlace.Application.Contracts.Abstractions.Persistence;

public interface ISchoolRepository
{
    Task<bool> ExistsAsync(int schoolId, CancellationToken cancellationToken = default);
}
