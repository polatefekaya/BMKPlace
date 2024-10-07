using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Services.Database;

public interface IUserDbService
{
    public Task<UserEntity> GetByPixelId();
    public Task<UserEntity> DeleteByPixelId();
    public Task<UserEntity> Delete();
    public Task<UserEntity[]> DeleteManyByCanvasId();
    public Task<UserEntity[]> GetManyByCanvasId();
}
