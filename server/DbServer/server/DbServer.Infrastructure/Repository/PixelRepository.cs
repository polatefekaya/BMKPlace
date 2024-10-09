using System;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Repository;

public class PixelRepository : IPixelRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPixelQueryService _pixelQueryService;

    public PixelRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory){
        _unitOfWork = unitOfWork;
        _pixelQueryService = sqlServiceFactory.Create().PixelQueryService();
    }
    public Task<PixelEntity> Add()
    {
        string sql = _pixelQueryService.Add;
        throw new NotImplementedException();
    }

    public Task<PixelEntity> Delete()
    {
        string sql = _pixelQueryService.DeleteById;
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> DeleteManyByColor()
    {
        string sql = _pixelQueryService.DeleteManyByColor;
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> DeleteManyByDate()
    {
        string sql = _pixelQueryService.DeleteManyByDate;
        throw new NotImplementedException();
    }

    public Task<PixelEntity> Get()
    {
        string sql = _pixelQueryService.GetById;
        throw new NotImplementedException();
    }

    public Task<PixelEntity> GetByPosition()
    {
        string sql = _pixelQueryService.GetManyByPosition;
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> GetManyByColor()
    {
        string sql = _pixelQueryService.GetManyByColor;
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> GetManyByDate()
    {
        string sql = _pixelQueryService.GetManyByDate;
        throw new NotImplementedException();
    }
}
