using System;
using System.Reflection.Metadata;
using Microsoft.Extensions.Logging;
using SRWorkerServer.GridCache.Data.Entities;
using SRWorkerServer.GridCache.Interfaces;

namespace SRWorkerServer.GridCache.Services;

public class CacheRepository : ICacheRepository
{
    private readonly ILogger<CacheRepository> _logger;
    private readonly GridDictionary _cache;

    public CacheRepository(ILogger<CacheRepository> logger, GridDictionary cache)
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
            _logger.LogError("No grid found");
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

    public async Task DeleteGridAsync(string gridName)
    {
        if (!_cache.ContainsKey(gridName))
        {
            _logger.LogError("No grid found");
            return;
        }

        bool gridRemoved = _cache.TryRemove(gridName);

        if (gridRemoved)
        {
            _logger.LogInformation("Grid Succesfully Deleted");
            return;
        }

        _logger.LogError("Grid Can't Be Deleted");
    }

    public async Task ResetPointAsync(string gridName, int posX, int posY)
    {
        if (!_cache.ContainsKey(gridName))
        {
            _logger.LogError("No grid found");
            return;
        }

        //bool pointResetted = _cache.Try
    }

    public Task<GridEntity?> GetGridAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<byte?> GetPointAsync(string gridName, int posX, int posY)
    {
        throw new NotImplementedException();
    }

    public async Task<byte[]?> GetAllPoints(string gridName){
        if (!_cache.ContainsKey(gridName))
        {
            _logger.LogError("No grid found, returning null");
            return null;
        }

        byte[]? points = _cache.TryGetAllPoints(gridName);
        if(points is null){
            _logger.LogWarning("Retrieved points is null");
            return null;
        }

        return points;
    }

    public async Task<bool> GridExists(string gridName)
    {
        return _cache.ContainsKey(gridName);
    }

    public Task UpdateGridAsync(string gridName, GridEntity grid)
    {
        throw new NotImplementedException();
    }

    public Task UpdatePointAsync(string gridName, int posX, int posY, byte point)
    {
        throw new NotImplementedException();
    }
}
