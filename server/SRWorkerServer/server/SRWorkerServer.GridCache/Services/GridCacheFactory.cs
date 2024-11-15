using System;
using SRWorkerServer.GridCache.Data.Entities;
using SRWorkerServer.GridCache.Interfaces;

namespace SRWorkerServer.GridCache.Services;

public class GridCacheFactory : IGridCacheFactory
{
    public Task<GridEntity> CreateGrid(int sizeX, int sizeY)
    {
        throw new NotImplementedException();
    }
}
