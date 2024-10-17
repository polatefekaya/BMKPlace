using System;
using Dapper;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;
using Microsoft.Extensions.Logging;

namespace DbServer.Infrastructure.Repository;

public class ContributionRepository : IContributionRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IContributionQueryService _queryService;
    private readonly ILogger<ContributionRepository> _logger;
    private readonly IRepositoryHelper _helper;
    private readonly string _className;
    public ContributionRepository(ILogger<ContributionRepository> logger, IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory, IRepositoryHelper helper)
    {
        _unitOfWork = unitOfWork;
        _queryService = sqlServiceFactory.Create().ContributionQueryService();
        _logger = logger;

        _helper = helper;
        _className = this.GetType().Name;
    }

    public async Task<DatabaseResult<ContributionEntity>> Add(ContributionEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<ContributionEntity> result = await _helper.ExecuteAsync<ContributionEntity>(async () => {
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

    public async Task<DatabaseResult<ContributionEntity>> Delete(int id){
        string methodName = nameof(Delete);
 
        DatabaseResult<ContributionEntity> result = await _helper.ExecuteAsync<ContributionEntity>(async () => {
            string sql = _queryService.DeleteById;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {Id = id},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByCanvasId(int canvasId) {
        string methodName = nameof(DeleteManyByCanvasId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.ExecuteAsync<IEnumerable<ContributionEntity>>(async () => {
            string sql = _queryService.DeleteManyByCanvasId;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {CanvasId = canvasId},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByPixelId(int pixelId)
    {
        string methodName = nameof(DeleteManyByPixelId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.ExecuteAsync<IEnumerable<ContributionEntity>>(async () => {
            string sql = _queryService.DeleteManyByPixelId;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {PixelId = pixelId},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<ContributionEntity>> DeleteByPositionWithCanvasId(string position, int canvasId)
    {
        string methodName = nameof(DeleteByPositionWithCanvasId);

        DatabaseResult<ContributionEntity> result = await _helper.ExecuteAsync<ContributionEntity>(async () => {
            string sql = _queryService.DeleteByPositionWithCanvasId;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {Position = position, CanvasId = canvasId},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByUserId(int userId)
    {
        string methodName = nameof(DeleteManyByUserId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.ExecuteAsync<IEnumerable<ContributionEntity>>(async () => {
            string sql = _queryService.DeleteManyByUserId;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {UserId = userId},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<ContributionEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<ContributionEntity> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetById;
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ContributionEntity>(
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

    public async Task<DatabaseResult<ContributionEntity>> GetByPositionWithCanvasId(string position, int canvasId)
    {
        string methodName = nameof(GetByPositionWithCanvasId);

        DatabaseResult<ContributionEntity> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetByPositionWithCanvasId;
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ContributionEntity>(
                sql,
                new {Position = position, CanvasId = canvasId},
                _unitOfWork.Transaction
            );
        },
            methodName,
            _className
        );
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByCanvasId(int canvasId)
    {
        string methodName = nameof(GetManyByCanvasId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetManyByCanvasId;
            return await _unitOfWork.Connection.QueryAsync<ContributionEntity>(
                sql,
                new {CanvasId = canvasId},
                _unitOfWork.Transaction
            );
        },
            methodName,
            _className
        );
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByPixelId(int pixelId)
    {
        string methodName = nameof(GetManyByPixelId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetManyByPixelId;
            return await _unitOfWork.Connection.QueryAsync<ContributionEntity>(
                sql,
                new {PixelId = pixelId},
                _unitOfWork.Transaction
            );
        },
            methodName,
            _className
        );
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByUserId(int userId)
    {
        string methodName = nameof(GetManyByUserId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetManyByUserId;
            return await _unitOfWork.Connection.QueryAsync<ContributionEntity>(
                sql,
                new {UserId = userId},
                _unitOfWork.Transaction
            );
        },
            methodName,
            _className
        );
        return result;
    }
}
