using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface IContributionRepository
{
    public Task<ContributionEntity> Get();
    public Task<ContributionEntity> Add();
    public Task<ContributionEntity> Delete();
    public Task<ContributionEntity[]> GetMany();
    public Task<ContributionEntity[]> DeleteMany();
    public Task<ContributionEntity> GetByPosition();
    public Task<ContributionEntity[]> GetManyByUserId();
    public Task<ContributionEntity[]> GetManyByCanvasId();
    public Task<ContributionEntity[]> GetManyByPixelId();
    public Task<ContributionEntity[]> DeleteManyByUserId();
    public Task<ContributionEntity[]> DeleteManyByCanvasId();
    public Task<ContributionEntity[]> DeleteManyByPixelId();
}
