using System;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Infrastructure.Services;

public class CanvasDbService : ICanvasDbService
{
    public Task<DatabaseResult<CanvasEntity>> Add()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<CanvasEntity>> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<CanvasEntity>>> DeleteManyByUserId()
    {
        throw new NotImplementedException();
    }

    public Task GetContributors()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<UserEntity>> GetCreator()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<CanvasEntity>>> GetManyByUserId()
    {
        throw new NotImplementedException();
    }
}
