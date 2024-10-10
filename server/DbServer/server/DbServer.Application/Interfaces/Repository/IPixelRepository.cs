using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface IPixelRepository
{
    public Task<DatabaseResult<PixelEntity>> Get(int id);
    public Task<DatabaseResult<PixelEntity>> Add();
    public Task<DatabaseResult<PixelEntity>> Delete();
    public Task<DatabaseResult<PixelEntity>> GetByPosition();
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByDate();
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByColor();
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByDate();
    public Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByColor();
}
