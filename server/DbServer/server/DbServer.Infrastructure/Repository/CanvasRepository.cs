using System;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Infrastructure.Repository;

public class CanvasRepository : ICanvasRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICanvasQueryService _canvasQueryService;
    public CanvasRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory)
    {
        _unitOfWork = unitOfWork;
        _canvasQueryService = sqlServiceFactory.Create().CanvasQueryService();
    }

    public Task<DatabaseResult<CanvasEntity>> Add()
    {
        string sql = _canvasQueryService.Add;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<CanvasEntity>> Delete()
    {
        string sql = _canvasQueryService.DeleteById;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<CanvasEntity>>> DeleteMany()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<CanvasEntity>> Get()
    {
        string sql = _canvasQueryService.GetById;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<CanvasEntity>>> GetMany()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<CanvasEntity>> Update()
    {
        string sql = _canvasQueryService.UpdateById;
        throw new NotImplementedException();
    }
}
