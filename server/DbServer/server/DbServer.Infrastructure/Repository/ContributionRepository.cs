using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Repository;

public class ContributionRepository : IContributionRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public ContributionRepository(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }
    public Task<ContributionEntity> Add()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity[]> DeleteMany()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity[]> DeleteManyByCanvasId()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity[]> DeleteManyByPixelId()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity[]> DeleteManyByUserId()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity> Get()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity> GetByPosition()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity[]> GetMany()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity[]> GetManyByCanvasId()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity[]> GetManyByPixelId()
    {
        throw new NotImplementedException();
    }

    public Task<ContributionEntity[]> GetManyByUserId()
    {
        throw new NotImplementedException();
    }
}
