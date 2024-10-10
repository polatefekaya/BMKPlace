using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface ICanvasRepository
{
    public Task<DatabaseResult<CanvasEntity>> Get();
    public Task<DatabaseResult<CanvasEntity>> Add();
    public Task<DatabaseResult<CanvasEntity>> Delete();
    public Task<DatabaseResult<CanvasEntity>> Update();
    public Task<DatabaseResult<IEnumerable<CanvasEntity>>> GetMany();
    public Task<DatabaseResult<IEnumerable<CanvasEntity>>> DeleteMany();
}
