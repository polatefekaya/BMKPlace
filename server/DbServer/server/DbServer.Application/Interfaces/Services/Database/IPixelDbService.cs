using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Services.Database;

public interface IPixelDbService
{
    public Task<PixelEntity[]> GetManyByCanvasId();
    public Task<PixelEntity[]> GetManyByUserId();
    public Task<PixelEntity[]> DeleteManyByUserId();
    public Task<PixelEntity[]> DeleteManyByCanvasId();
    public Task<PixelEntity> Add();
    public Task<PixelEntity[]> DeleteManyByDate();
    public Task<PixelEntity> Delete();
    public Task<PixelEntity[]> DeleteManyByColor();
    
}
