using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Services.Database;

public interface IUserDbService
{
    public Task<DatabaseResult<UserEntity>> GetByPixelId();
    public Task<DatabaseResult<UserEntity>> DeleteByPixelId();
    public Task<DatabaseResult<UserEntity>> Delete();
    public Task DeleteManyByCanvasId();
    public Task GetManyByCanvasId();
}
