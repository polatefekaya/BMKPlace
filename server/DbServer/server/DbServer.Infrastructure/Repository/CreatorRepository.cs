using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Repository;

public class CreatorRepository : ICreatorRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatorRepository(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }
    public Task<CreationEntity> Add()
    {
        throw new NotImplementedException();
    }

    public Task<CreationEntity> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<CreationEntity[]> DeleteMany()
    {
        throw new NotImplementedException();
    }

    public Task<CreationEntity> Get()
    {
        throw new NotImplementedException();
    }

    public Task<CreationEntity[]> GetMany()
    {
        throw new NotImplementedException();
    }
}
