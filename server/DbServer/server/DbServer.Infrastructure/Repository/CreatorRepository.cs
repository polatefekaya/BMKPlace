using System;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Repository;

public class CreatorRepository : ICreatorRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICreationQueryService _creationQueryService;

    public CreatorRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory){
        _unitOfWork = unitOfWork;
        _creationQueryService = sqlServiceFactory.Create().CreationQueryService();
    }
    public Task<CreationEntity> Add()
    {
        string sql = _creationQueryService.Add;
        throw new NotImplementedException();
    }

    public Task<CreationEntity> Delete()
    {
        string sql = _creationQueryService.DeleteById;
        throw new NotImplementedException();
    }

    public Task<CreationEntity[]> DeleteMany()
    {
        throw new NotImplementedException();
    }

    public Task<CreationEntity> Get()
    {
        string sql = _creationQueryService.GetById;
        throw new NotImplementedException();
    }

    public Task<CreationEntity[]> GetMany()
    {
        throw new NotImplementedException();
    }
}
