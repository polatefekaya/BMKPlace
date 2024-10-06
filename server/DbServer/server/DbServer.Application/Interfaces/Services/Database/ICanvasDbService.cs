using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Services.Database;

public interface ICanvasDbService
{
    public Task<CanvasEntity> Add();
    public Task<CanvasEntity> Delete();
    public Task<UserEntity> GetCreator();
    public Task<CanvasEntity[]> GetManyByUserId();
    public Task<CanvasEntity[]> DeleteManyByUserId();
    public Task<UserEntity[]> GetContributors();
}
