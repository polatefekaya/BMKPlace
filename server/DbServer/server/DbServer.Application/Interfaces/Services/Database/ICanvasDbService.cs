using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Services.Database;

public interface ICanvasDbService
{
    public Task<DatabaseResult<CanvasEntity>> Add();
    public Task<DatabaseResult<CanvasEntity>> Delete();
    public Task<DatabaseResult<UserEntity>> GetCreator();
    public Task<DatabaseResult<IEnumerable<CanvasEntity>>> GetManyByUserId();
    public Task<DatabaseResult<IEnumerable<CanvasEntity>>> DeleteManyByUserId();
    public Task GetContributors();
}
