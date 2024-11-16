using System;

namespace SRWorkerServer.GridCache.Interfaces;

public interface IPointCacheRepository
{
    public Task AddPointAsync(string gridName, byte point, int posX, int posY);
    public Task<byte?> GetPointAsync(string gridName, int posX, int posY);
    public Task<byte[]?> GetAllPoints(string gridName);
    public Task UpdatePointAsync(string gridName, int posX, int posY, byte point);
    public Task ResetPointAsync(string gridName, int posX, int posY);

}
