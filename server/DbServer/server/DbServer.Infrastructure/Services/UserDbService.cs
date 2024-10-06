using System;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Services;

public class UserDbService : IUserDbService
{
    public Task<UserEntity> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity> DeleteByPixelId()
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity[]> DeleteManyByCanvasId()
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity> GetByPixelId()
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity[]> GetManyByCanvasId()
    {
        throw new NotImplementedException();
    }
}
