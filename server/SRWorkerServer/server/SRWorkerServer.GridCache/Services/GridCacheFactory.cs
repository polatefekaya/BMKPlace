using System;
using Microsoft.Extensions.Logging;
using SRWorkerServer.GridCache.Data.Entities;
using SRWorkerServer.GridCache.Interfaces;

namespace SRWorkerServer.GridCache.Services;

public class GridCacheFactory : IGridCacheFactory
{
    private readonly ILogger<GridCacheFactory> _logger;
    public GridCacheFactory(ILogger<GridCacheFactory> logger){
        _logger = logger;
    }
    public async Task<GridEntity> CreateGrid(int sizeX, int sizeY)
    {
        _logger.LogInformation("Creating new grid in {className}", nameof(GridCacheFactory));
        _logger.LogDebug("SizeX: {sizeX}, SizeY: {sizeY}", sizeX, sizeY);
        
        return new GridEntity {
            SizeX = sizeX,
            SizeY = sizeY
        };
    }
}
