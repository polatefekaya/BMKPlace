using System;
using Microsoft.Extensions.Logging;
using SRWorkerServer.GridCache.Data.Entities;
using SRWorkerServer.GridCache.Interfaces;

namespace SRWorkerServer.GridCache.Services;

public class CacheService : ICacheService
{
    private readonly ILogger<CacheService> _logger;
    private readonly IGridCacheRepository _repository;
    private readonly IGridCacheFactory _cacheFactory;

    public CacheService(ILogger<CacheService> logger, IGridCacheRepository repository, IGridCacheFactory cacheFactory){
        _logger = logger;
        _repository = repository;
        _cacheFactory = cacheFactory;
    }
    public async Task CacheByte(string gridName, byte value, int posX, int posY)
    {
        GridEntity? entity = await _repository.GetGridAsync(gridName);

        if (entity is null){
            entity = await _cacheFactory.CreateGrid(999,999);
            await _repository.AddGridAsync(gridName, entity);
        }

        //TODO cache point(byte)
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
