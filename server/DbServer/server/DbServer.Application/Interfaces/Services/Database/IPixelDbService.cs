using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Services.Database;

public interface IPixelDbService
{
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByCanvasId();
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByUserId();
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByUserId();
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByCanvasId();
    public Task<DatabaseResult<PixelEntity>> Add();
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByDate();
    public Task<DatabaseResult<PixelEntity>> Delete();
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByColor();

}
