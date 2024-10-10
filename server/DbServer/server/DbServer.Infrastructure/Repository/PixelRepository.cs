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
    private readonly IPixelQueryService _pixelQueryService;
    private readonly ILogger<PixelRepository> _logger;

    public PixelRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory, ILogger<PixelRepository> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _pixelQueryService = sqlServiceFactory.Create().PixelQueryService();
    }
    public Task<DatabaseResult<PixelEntity>> Add()
    {
        string sql = _pixelQueryService.Add;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<PixelEntity>> Delete()
    {
        string sql = _pixelQueryService.DeleteById;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByColor()
    {
        string sql = _pixelQueryService.DeleteManyByColor;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByDate()
    {
        string sql = _pixelQueryService.DeleteManyByDate;
        throw new NotImplementedException();
    }

    public async Task<DatabaseResult<PixelEntity>> Get(int id)
    {
        _unitOfWork.Begin();
        _logger.LogDebug("Pixel Get Transaction Began");

        string sql = _pixelQueryService.GetById;
        PixelEntity? entity = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<PixelEntity>(
            sql,
            new { Id = id },
            _unitOfWork.Transaction);

        if (entity is null)
        {
            _logger.LogWarning("Pixel Entity not found");

            _unitOfWork.Rollback();
            _logger.LogDebug("Pixel Get Transaction Rolledback");
            
            return DatabaseResult<PixelEntity>.Error("Pixel Entity not found", ErrorTypes.NotFound);
        }

        _unitOfWork.Commit();
        _logger.LogDebug("Pixel Get Transaction Committed");

        _logger.LogInformation("Pixel Entity found");
        return DatabaseResult<PixelEntity>.Success(entity);
    }

    public Task<DatabaseResult<PixelEntity>> GetByPosition()
    {
        string sql = _pixelQueryService.GetManyByPosition;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByColor()
    {
        string sql = _pixelQueryService.GetManyByColor;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByDate()
    {
        string sql = _pixelQueryService.GetManyByDate;
        throw new NotImplementedException();
    }
}
