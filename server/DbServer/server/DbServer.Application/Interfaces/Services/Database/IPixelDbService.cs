using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Services.Database;

public interface IPixelDbService : IPixelRepository
{
    // public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByCanvasId();
    // public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByUserId();
    // public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByUserId();
    // public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByCanvasId();
    // public Task<DatabaseResult<PixelEntity>> GetByPosition(string position, int canvasId);

}
