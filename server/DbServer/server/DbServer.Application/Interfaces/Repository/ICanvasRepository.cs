using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface ICanvasRepository
{
    public Task<DatabaseResult<CanvasEntity>> Get(int id);
    public Task<DatabaseResult<CanvasEntity>> Add(CanvasEntity entity);
    public Task<DatabaseResult<CanvasEntity>> Delete(int id);
    public Task<DatabaseResult<CanvasEntity>> Update(CanvasEntity entity);
}
