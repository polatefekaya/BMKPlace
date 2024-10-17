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

public class UserRepository : IUserRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserQueryService _queryService;
    private readonly ILogger<UserRepository> _logger;
    private readonly IRepositoryHelper _helper;
    private readonly string _className;
    public UserRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory, ILogger<UserRepository> logger, IRepositoryHelper helper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;

        _helper = helper;
        _className = this.GetType().Name;
        _queryService = sqlServiceFactory.Create().UserQueryService();
    }
    public async Task<DatabaseResult<UserEntity>> Add(UserEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<UserEntity> result = await _helper.ExecuteAsync<UserEntity>(async () => {
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

    public async Task<DatabaseResult<UserEntity>> Delete(int id)
    {
        string methodName = nameof(Delete);

        DatabaseResult<UserEntity> result = await _helper.ExecuteAsync<UserEntity>(async () => {
            string sql = _queryService.DeleteById;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {Id = id},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<UserEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<UserEntity> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetById;
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<UserEntity>(
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

    public async Task<DatabaseResult<UserEntity>> Update(UserEntity entity)
    {
        string methodName = nameof(Update);

        DatabaseResult<UserEntity> result = await _helper.ExecuteAsync<UserEntity>(async () => {
            string sql = _queryService.UpdateById;
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
}
