using System;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BMKPlace.Infrastructure.Persistence.Repositories;

internal sealed class SchoolRepository : ISchoolRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SchoolRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(int schoolId, CancellationToken cancellationToken = default)
    {
        // Efficiently check if any entity with the given primary key exists
        return await _dbContext.Schools.AnyAsync(s => s.Id == schoolId, cancellationToken);
    }
}
