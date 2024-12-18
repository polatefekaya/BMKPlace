using System;
using Microsoft.Extensions.Logging;
using SRWorkerServer.GridCache.Data.Entities;
using SRWorkerServer.GridCache.Interfaces;

namespace SRWorkerServer.GridCache.Services;

public class CacheService : ICacheService
{
    private readonly ILogger<CacheService> _logger;
    private readonly ICacheRepository _repository;
    private readonly IGridCacheFactory _cacheFactory;

    private readonly string _className;

    public CacheService(ILogger<CacheService> logger, ICacheRepository repository, IGridCacheFactory cacheFactory)
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

        if (!exists)
        {
            _logger.LogWarning("Grid is not exists in current cache, creating new and adding it to cache");
            _logger.LogDebug("Creating with\n SizeX: {sizeX}, SizeY: {sizeY}", 999, 999);

            GridEntity grid = await _cacheFactory.CreateGrid(999, 999);
            await _repository.AddGridAsync(gridName, grid);
            _logger.LogDebug("Grid created and added");
        }

        await _repository.AddPointAsync(gridName, value, posX, posY);
        _logger.LogInformation("{methodName} is finished in {className}", nameof(CacheByte), _className);
    }

    public async Task<byte[]?> GetAllBytes(string gridName)
    {
        _logger.LogInformation("{methodName} is starting in {className}", nameof(GetAllBytes), _className);
        _logger.LogDebug("Grid Name: {gridName}", gridName);

        bool exists = await _repository.GridExists(gridName);

        if (!exists)
        {
            _logger.LogError("Grid is not exists, returning null");
            return null;
        }

        byte[]? bytes = await _repository.GetAllPoints(gridName);
        if(bytes is null){
            _logger.LogWarning("Retrieved points from repository is null, returning null");
            return null;
        }

        _logger.LogInformation("{methodName} is finished in {className}", nameof(GetAllBytes), _className);
        return bytes;
    }

    public async Task<byte?> GetByte(string gridName, int posX, int posY)
    {
        _logger.LogInformation("{methodName} is starting in {className}", nameof(GetByte), _className);
        _logger.LogDebug("Grid Name: {gridName}\nPosX: {posX}, PosY: {posY}",
            gridName,
            posX, posY
        );
        bool exists = await _repository.GridExists(gridName);

        if (!exists)
        {
            _logger.LogError("Grid is not exists, returning null");
            return null;
        }

        byte? point = await _repository.GetPointAsync();
        _logger.LogDebug("Point succesfully got, Value: {value}", point);

        _logger.LogInformation("{methodName} is finished in {className}", nameof(GetByte), _className);
        return point;
    }

    public async Task<GridEntity?> GetGrid(string gridName)
    {
        _logger.LogInformation("{methodName} is starting in {className}", nameof(GetGrid), _className);
        _logger.LogDebug("Grid Name: {gridName}", gridName);

        bool exists = await _repository.GridExists(gridName);

        if (!exists)
        {
            _logger.LogError("Grid is not exists, returning null");
            return null;
        }

        //TODO get grid logic here
        GridEntity? grid = await _repository.GetGridAsync(gridName);
        if (grid is null){
            _logger.LogWarning("Retrieved grid is null, returning null");
        }

        _logger.LogInformation("{methodName} is finished in {className}", nameof(GetGrid), _className);
        return grid;
    }

    public async Task ResetByte(string gridName, int posX, int posY)
    {
        _logger.LogInformation("{methodName} is starting in {className}", nameof(ResetByte), _className);
        _logger.LogDebug("Grid Name: {gridName}\nPosX: {posX}, PosY: {posY}",
            gridName,
            posX, posY
        );
        bool exists = await _repository.GridExists(gridName);

        if (!exists)
        {
            _logger.LogError("Grid is not exists, returning");
            return;
        }

        //TODO finish the deletePointAsync method
        await _repository.DeletePointAsync();
        _logger.LogDebug("Point succesfully resetted");

        _logger.LogInformation("{methodName} is finished in {className}", nameof(ResetByte), _className);
    }
}
