using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface IPixelRepository
{
    public Task<DatabaseResult<PixelEntity>> Get(int id);
    public Task<DatabaseResult<PixelEntity>> Add(PixelEntity entity);
    public Task<DatabaseResult<PixelEntity>> Delete(int id);
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByDate(string date);
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByColor(string color);
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByDate(string date);
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByColor(string color);
}
