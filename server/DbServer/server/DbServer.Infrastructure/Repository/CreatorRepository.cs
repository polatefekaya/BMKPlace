using System;
using Dapper;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Models.Errors;
using DbServer.Domain.Data.Results;
using Microsoft.Extensions.Logging;

namespace DbServer.Infrastructure.Repository;

public class CreatorRepository : ICreatorRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICreationQueryService _queryService;
    private readonly ILogger<CreatorRepository> _logger;
    private readonly IRepositoryHelper _helper;
    private readonly string _className;

    public CreatorRepository(ILogger<CreatorRepository> logger, IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory, IRepositoryHelper helper)
    {
        _unitOfWork = unitOfWork;
        _queryService = sqlServiceFactory.Create().CreationQueryService();
        _logger = logger;

        _helper = helper;
        _className = this.GetType().Name;
    }

    public async Task<DatabaseResult<CreationEntity>> Add(CreationEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<CreationEntity> result = await _helper.ExecuteAsync<CreationEntity>(async () => {
            string sql = _queryService.Add;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                entity,
                _unitOfWork.Transaction
            );
        },
            methodName,
            _className
        );
        return result;
    }

    public async Task<DatabaseResult<CreationEntity>> Delete(int id)
    {
        string methodName = nameof(Delete);

        DatabaseResult<CreationEntity> result = await _helper.ExecuteAsync<CreationEntity>(async () => {
            string sql = _queryService.DeleteById;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {Id = id},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<CreationEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<CreationEntity> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetById;
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<CreationEntity>(
                sql,
                new {Id = id},
                _unitOfWork.Transaction
            );
        },
            methodName,
            _className
        );
        return result;
    }
}
