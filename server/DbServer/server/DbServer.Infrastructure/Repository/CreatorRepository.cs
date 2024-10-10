using System;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Infrastructure.Repository;

public class CreatorRepository : ICreatorRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICreationQueryService _creationQueryService;

    public CreatorRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory)
    {
        _unitOfWork = unitOfWork;
        _creationQueryService = sqlServiceFactory.Create().CreationQueryService();
    }
    public Task<DatabaseResult<CreationEntity>> Add()
    {
        string sql = _creationQueryService.Add;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<CreationEntity>> Delete()
    {
        string sql = _creationQueryService.DeleteById;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<CreationEntity>>> DeleteMany()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<CreationEntity>> Get()
    {
        string sql = _creationQueryService.GetById;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<CreationEntity>>> GetMany()
    {
        throw new NotImplementedException();
    }
}
