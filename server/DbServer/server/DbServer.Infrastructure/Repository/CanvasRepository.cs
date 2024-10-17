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

public class CanvasRepository : ICanvasRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICanvasQueryService _queryService;
    private readonly ILogger<CanvasRepository> _logger;
    private readonly IRepositoryHelper _helper;
    private readonly string _className;
    public CanvasRepository(ILogger<CanvasRepository> logger, IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory, IRepositoryHelper helper)
    {
        _unitOfWork = unitOfWork;
        _queryService = sqlServiceFactory.Create().CanvasQueryService();
        _logger = logger;

        _helper = helper;
        _className = this.GetType().Name;
    }

    public async Task<DatabaseResult<CanvasEntity>> Add(CanvasEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<CanvasEntity> result = await _helper.ExecuteAsync<CanvasEntity>(async () => {
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

    public async Task<DatabaseResult<CanvasEntity>> Delete(int id)
    {
        string methodName = nameof(Delete);

        DatabaseResult<CanvasEntity> result = await _helper.ExecuteAsync<CanvasEntity>(async () => {
            string sql = _queryService.DeleteById;
            return await _unitOfWork.Connection.ExecuteAsync(
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

    public async Task<DatabaseResult<CanvasEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<CanvasEntity> result = await _helper.QueryAsync<CanvasEntity>(async () => {
            string sql = _queryService.GetById;
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<CanvasEntity>(
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

    public async Task<DatabaseResult<CanvasEntity>> Update(CanvasEntity entity)
    {
        string methodName = nameof(Update);

        DatabaseResult<CanvasEntity> result = await _helper.ExecuteAsync<CanvasEntity>(async () => {
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
