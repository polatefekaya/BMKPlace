using System;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Services;

public class PixelDbService : IPixelDbService
{
    public Task<PixelEntity> Add()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> DeleteManyByCanvasId()
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

    public Task<PixelEntity[]> DeleteManyByUserId()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> GetManyByCanvasId()
    {
        throw new NotImplementedException();
    }

    public Task<PixelEntity[]> GetManyByUserId()
    {
        throw new NotImplementedException();
    }
}
