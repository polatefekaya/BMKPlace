using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface IPixelRepository
{
    public Task<PixelEntity> Get();
    public Task<PixelEntity> Add();
    public Task<PixelEntity> Delete();
    public Task<PixelEntity> GetByPosition();
    public Task<PixelEntity[]> GetManyByDate();
    public Task<PixelEntity[]> GetManyByColor();
    public Task<PixelEntity[]> DeleteManyByDate();
    public Task<PixelEntity[]> DeleteManyByColor();
}
