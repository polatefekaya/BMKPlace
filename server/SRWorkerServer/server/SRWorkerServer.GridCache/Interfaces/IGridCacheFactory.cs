using System;
using SRWorkerServer.GridCache.Data.Entities;

namespace SRWorkerServer.GridCache.Interfaces;

public interface IGridCacheFactory
{
    public Task<GridEntity> CreateGrid(int sizeX, int sizeY);
}
