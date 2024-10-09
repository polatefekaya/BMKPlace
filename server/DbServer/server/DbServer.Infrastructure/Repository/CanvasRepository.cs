using System;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Repository;

public class CanvasRepository : ICanvasRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICanvasQueryService _canvasQueryService;
    public CanvasRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory){
        _unitOfWork = unitOfWork;
        _canvasQueryService = sqlServiceFactory.Create().CanvasQueryService();
    }

    public Task<CanvasEntity> Add()
    {
        string sql = _canvasQueryService.Add;
        throw new NotImplementedException();
    }

    public Task<CanvasEntity> Delete()
    {
        string sql = _canvasQueryService.DeleteById;
        throw new NotImplementedException();
    }

    public Task<CanvasEntity[]> DeleteMany()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity> Get()
    {
        string sql = _canvasQueryService.GetById;
        throw new NotImplementedException();
    }

    public Task<CanvasEntity[]> GetMany()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity> Update()
    {
        string sql = _canvasQueryService.UpdateById;
        throw new NotImplementedException();
    }
}
