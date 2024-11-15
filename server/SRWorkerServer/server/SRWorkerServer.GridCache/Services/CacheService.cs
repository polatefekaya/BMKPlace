using System;
using Microsoft.Extensions.Logging;
using SRWorkerServer.GridCache.Data.Entities;
using SRWorkerServer.GridCache.Interfaces;

namespace SRWorkerServer.GridCache.Services;

public class CacheService : ICacheService
{
    private readonly ILogger<CacheService> _logger;
    private readonly IGeneralCacheRepository _repository;
    private readonly IGridCacheFactory _cacheFactory;

    private readonly string _className;

    public CacheService(ILogger<CacheService> logger, IGeneralCacheRepository repository, IGridCacheFactory cacheFactory)
    {
        _logger = logger;
        _repository = repository;
        _cacheFactory = cacheFactory;

        _className = nameof(CacheService);
    }
    public async Task CacheByte(string gridName, byte value, int posX, int posY)
    {
        _logger.LogInformation("{methodName} is starting in {className}", nameof(CacheByte), _className);
        _logger.LogDebug("Grid Name: {gridName}\nValue: {value}\nPosX: {posX}, PosY: {posY}",
            gridName,
            value,
            posX, posY    
        );
        bool exists = await _repository.GridExists(gridName);

        if (!exists) {
            _logger.LogWarning("Grid is not exists in current cache, creating new and adding it to cache");
            _logger.LogDebug("Creating with\n SizeX: {sizeX}, SizeY: {sizeY}", 999, 999);

            GridEntity grid = await _cacheFactory.CreateGrid(999, 999);
            await _repository.AddGridAsync(gridName, grid);
            _logger.LogDebug("Grid created and added");
        }
        
        await _repository.AddPointAsync(gridName, value, posX, posY);
        _logger.LogInformation("{methodName} is finished in {className}", nameof(CacheByte), _className);
    }

    public Task<byte> GetByte(string gridName, int posX, int posY)
    {
        throw new NotImplementedException();
    }

    public Task ResetByte(string gridName, int posX, int posY)
    {
        throw new NotImplementedException();
    }
}
