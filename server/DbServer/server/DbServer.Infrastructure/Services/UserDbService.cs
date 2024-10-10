using System;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Infrastructure.Services;

public class UserDbService : IUserDbService
{
    public Task<DatabaseResult<UserEntity>> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<UserEntity>> DeleteByPixelId()
    {
        throw new NotImplementedException();
    }

    public Task DeleteManyByCanvasId()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<UserEntity>> GetByPixelId()
    {
        throw new NotImplementedException();
    }

    public Task GetManyByCanvasId()
    {
        throw new NotImplementedException();
    }
}
