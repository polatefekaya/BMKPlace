using System;
using SRWorkerServer.GridCache.Data.Entities;

namespace SRWorkerServer.GridCache.Interfaces;

public interface IGridCacheRepository
{
    public Task AddGridAsync(string name, GridEntity? entity);
    public Task<GridEntity?> GetGridAsync(string name);
    public Task UpdateGridAsync();
    public Task DeleteGridAsync();
    public Task<bool> GridExists(string gridName);
}
