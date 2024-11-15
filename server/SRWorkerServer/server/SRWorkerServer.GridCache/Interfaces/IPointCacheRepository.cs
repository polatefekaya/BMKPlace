using System;

namespace SRWorkerServer.GridCache.Interfaces;

public interface IPointCacheRepository
{
    public Task AddPointAsync(string gridName, byte point, int posX, int posY);
    public Task<byte?> GetPointAsync();
    public Task<byte[]?> GetAllPoints(string gridName);
    public Task UpdatePointAsync();
    public Task DeletePointAsync();

}
