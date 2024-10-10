using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface IContributionRepository
{
    public Task<DatabaseResult<ContributionEntity>> Get();
    public Task<DatabaseResult<ContributionEntity>> Add();
    public Task<DatabaseResult<ContributionEntity>> Delete();
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetMany();
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteMany();
    public Task<DatabaseResult<ContributionEntity>> GetByPosition();
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByUserId();
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByCanvasId();
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByPixelId();
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByUserId();
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByCanvasId();
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByPixelId();
}
