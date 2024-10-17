using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Services.Database;

public interface ICanvasDbService : ICanvasRepository
{
    // public Task<DatabaseResult<UserEntity>> GetCreator();
    // public Task<DatabaseResult<IEnumerable<CanvasEntity>>> GetManyByUserId();
    // public Task<DatabaseResult<IEnumerable<CanvasEntity>>> DeleteManyByUserId();
    // public Task GetContributors();
}
