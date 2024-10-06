using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Repository;

public class PixelRepository : IPixelRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public PixelRepository(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }
    public Task<PixelEntity> Add()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> DeleteManyByColor()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> DeleteManyByDate()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity> Get()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity> GetByPosition()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> GetManyByColor()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> GetManyByDate()
    {
        throw new NotImplementedException();
    }
}
