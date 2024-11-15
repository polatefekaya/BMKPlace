using System;
using Microsoft.Extensions.Logging;
using SRWorkerServer.GridCache.Data.Entities;
using SRWorkerServer.GridCache.Interfaces;

namespace SRWorkerServer.GridCache.Services;

public class GeneralCacheRepository : IGeneralCacheRepository
{
    private readonly ILogger<GeneralCacheRepository> _logger;
    private readonly GridDictionary _cache;

    public GeneralCacheRepository(ILogger<GeneralCacheRepository> logger, GridDictionary cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task AddGridAsync(string name, GridEntity? entity)
    {
        bool added = _cache.TryAdd(name, entity ?? new());

        if (added)
        {
            _logger.LogInformation("Grid Succesfully Added");
            return;
        }

        _logger.LogError("Grid Can't Be Added");
    }

    public async Task AddPointAsync(string gridName, byte point, int posX, int posY)
    {
        if (!_cache.ContainsKey(gridName))
        {
            return;
        }
        bool pointAdded = _cache.TryAddPoint(gridName, posX, posY, point);

        if (pointAdded)
        {
            _logger.LogInformation("Point Succesfully Added");
            return;
        }

        _logger.LogError("Point Can't Be Added");
    }

    public Task DeleteGridAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeletePointAsync()
    {
        throw new NotImplementedException();
    }

    public Task<GridEntity?> GetGridAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<byte> GetPointAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> GridExists(string gridName)
    {
        return _cache.ContainsKey(gridName);
    }

    public Task UpdateGridAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdatePointAsync()
    {
        throw new NotImplementedException();
    }
}
