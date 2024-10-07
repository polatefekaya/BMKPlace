using System;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Services;

public class CanvasDbService : ICanvasDbService
{
    public Task<CanvasEntity> Add()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity[]> DeleteManyByUserId()
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity[]> GetContributors()
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity> GetCreator()
    {
        throw new NotImplementedException();
    }

    public Task<CanvasEntity[]> GetManyByUserId()
    {
        throw new NotImplementedException();
    }
}
