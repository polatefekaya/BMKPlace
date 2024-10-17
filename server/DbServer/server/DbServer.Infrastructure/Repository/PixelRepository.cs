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

public class PixelRepository : IPixelRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPixelQueryService _queryService;
    private readonly ILogger<PixelRepository> _logger;
    private readonly IRepositoryHelper _helper;
    private readonly string _className;
    public PixelRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory, ILogger<PixelRepository> logger, IRepositoryHelper helper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;

        _helper = helper;
        _className = this.GetType().Name;
        _queryService = sqlServiceFactory.Create().PixelQueryService();
    }

    public async Task<DatabaseResult<PixelEntity>> Add(PixelEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<PixelEntity> result = await _helper.ExecuteAsync<PixelEntity>(async () => {
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

    public async Task<DatabaseResult<PixelEntity>> Delete(int id)
    {
        string methodName = nameof(Delete);

        DatabaseResult<PixelEntity> result = await _helper.ExecuteAsync<PixelEntity>(async () => {
            string sql = _queryService.DeleteById;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {Id = id},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByColor(string color)
    {
        string methodName = nameof(DeleteManyByColor);

        DatabaseResult<IEnumerable<PixelEntity>> result = await _helper.ExecuteAsync<IEnumerable<PixelEntity>>(async () => {
            string sql = _queryService.DeleteManyByColor;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {Color = color},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByDate(string date)
    {
        string methodName = nameof(DeleteManyByDate);

        DatabaseResult<IEnumerable<PixelEntity>> result = await _helper.ExecuteAsync<IEnumerable<PixelEntity>>(async () => {
            string sql = _queryService.DeleteManyByDate;
            return await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new {Date = date},
                _unitOfWork.Transaction
            );
        }, methodName, _className);
        return result;
    }

    public async Task<DatabaseResult<PixelEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<PixelEntity> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetById;
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<PixelEntity>(
                sql,
                new {Id = id},
                _unitOfWork.Transaction
            );
        }, methodName, _className
        );
        return result;
    }

    public async Task<DatabaseResult<PixelEntity>> GetByPosition(string position, int canvasId)
    {
        string methodName = "GetByPositionWithCanvasId";

        DatabaseResult<PixelEntity> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetByPositionWithCanvasId;
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<PixelEntity>(
                sql,
                new {Position = position, CanvasId = canvasId},
                _unitOfWork.Transaction
            );
        }, methodName, _className
        );
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByColor(string color)
    {
        string methodName = nameof(GetManyByColor);

        DatabaseResult<IEnumerable<PixelEntity>> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetManyByColor;
            return await _unitOfWork.Connection.QueryAsync<PixelEntity>(
                sql,
                new {Color = color},
                _unitOfWork.Transaction
            );
        }, methodName, _className
        );
        return result;
    }

    public async Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByDate(string date)
    {
        string methodName = nameof(GetManyByDate);

        DatabaseResult<IEnumerable<PixelEntity>> result = await _helper.QueryAsync(async () => {
            string sql = _queryService.GetManyByDate;
            return await _unitOfWork.Connection.QueryAsync<PixelEntity>(
                sql,
                new {Date = date},
                _unitOfWork.Transaction
            );
        }, methodName, _className
        );
        return result;
    }
}
