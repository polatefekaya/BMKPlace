using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Services.Database;

public interface IUserDbService : IUserRepository
{
    // public Task<DatabaseResult<UserEntity>> GetByPixelId();
    // public Task<DatabaseResult<UserEntity>> DeleteByPixelId();
    // public Task DeleteManyByCanvasId();
    // public Task GetManyByCanvasId();
}
