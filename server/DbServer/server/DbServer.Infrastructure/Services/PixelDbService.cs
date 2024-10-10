using System;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Infrastructure.Services;

public class PixelDbService : IPixelDbService
{
    public Task<DatabaseResult<PixelEntity>> Add()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<PixelEntity>> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByCanvasId()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByColor()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByDate()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByUserId()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByCanvasId()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByUserId()
    {
        throw new NotImplementedException();
    }
}
