using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SRWorkerServer.GridCache.Data.Entities;

namespace SRWorkerServer.GridCache.Services;

public class GridDictionary
{
    private readonly ConcurrentDictionary<string, GridEntity> _internalDictionary;
    private readonly ILogger<GridDictionary> _logger;
    public GridDictionary(ILogger<GridDictionary> logger){
        _logger = logger;

        _internalDictionary = new ConcurrentDictionary<string, GridEntity>();
    }

    /// <summary>
    /// Tries to add the provided GridEntity to internal concurrent dictionary
    /// </summary>
    /// <param name="key">The name of the grid</param>
    /// <param name="gridEntity">The GridEntity to add</param>
    /// <returns><c>true</c> if the additinos is succesfull; otherwise <c>false</c></returns>
    public bool TryAdd(string key, GridEntity gridEntity)
    {
        if(string.IsNullOrWhiteSpace(key) || gridEntity is null){
            return false;
        }

        return _internalDictionary.TryAdd(key, gridEntity);
    }

    /// <summary>
    /// Tries to get the GridEntity from internal concurrent dictionary with given key
    /// </summary>
    /// <param name="key">The name of the grid</param>
    /// <param name="gridEntity">The GridEntity to got</param>
    /// <returns><c>true</c> if the getting is succesfull; otherwise <c>false</c></returns>
    public bool TryGetValue(string key, out GridEntity? gridEntity)
    {
        if(string.IsNullOrWhiteSpace(key)){
            gridEntity = null;
            return false;
        }

        return _internalDictionary.TryGetValue(key, out gridEntity);
    }

    /// <summary>
    /// Tries to add specified point with given coordinates and gridName (key) to GridEntity which is in the internal concurrent dictionary
    /// </summary>
    /// <param name="key">The name of the grid</param>
    /// <param name="posX">X coordinate of the point</param>
    /// <param name="posY">Y coordinate of the point</param>
    /// <param name="value">Color value of the point based on schema</param>
    /// <returns><c>true</c> if the additinos is succesfull; otherwise <c>false</c></returns>
    public bool TryAddPoint(string key, int posX, int posY, byte value)
    {
        if(string.IsNullOrWhiteSpace(key) || posX < 0 || posY < 0){
            return false;
        }

        if (_internalDictionary.TryGetValue(key, out var entity))
        {
            int pos = entity.CalculatePosition(posX, posY);
            bool isInBounds = pos >= 0 && pos < entity.Bytes.Length;
            
            if (!isInBounds) {
                return false;
            }
            
            entity.Bytes[pos] = value;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get specified point with given coordinates and gridName (key) from GridEntity which is in the internal concurrent dictionary
    /// </summary>
    /// <param name="key">The name of the grid</param>
    /// <param name="posX">X coordinate of the point</param>
    /// <param name="posY">Y coordinate of the point</param>
    /// <returns><c>(true, byte)</c> if the getting is succesfull; otherwise <c>(false, null)</c></returns>
    public (bool Success, byte? Values) TryGetPoint(string key, int posX, int posY){
        if(string.IsNullOrWhiteSpace(key) || posX < 0 || posY < 0){
            return (false, null);
        }

        if(_internalDictionary.TryGetValue(key, out var entity)){
            int pos = entity.CalculatePosition(posX, posY);
            bool isInBounds = pos >= 0 && pos < entity.Bytes.Length;

            if (!isInBounds){
                return (false, null);
            }

            return (true, entity.Bytes[pos]);
        }
        return (false, null);
    }

    /// <summary>
    /// Tries to get all points with specified gridName (key) from GridEntity which is in the internal concurrent dictionary
    /// </summary>
    /// <param name="key">The name of the grid</param>
    /// <returns><c>(true, byte[])</c> if the getting is succesfull; otherwise <c>(false, null)</c></returns>
    public (bool Success, byte[]? Values) TryGetAllPoints(string key){
        if(string.IsNullOrWhiteSpace(key)){
            return (false, null);
        }
        if(_internalDictionary.TryGetValue(key, out var entity)){
            return (true ,entity.Bytes.ToArray());
        }
        return (false, null);
    }

    // Additional thread-safe dictionary operations
    public bool TryRemove(string key)
    {
        return _internalDictionary.TryRemove(key, out _);
    }

    public bool ContainsKey(string key)
    {
        return _internalDictionary.ContainsKey(key);
    }

    public void Clear(){
        _internalDictionary.Clear();
    }

    public int Count => _internalDictionary.Count;
}
