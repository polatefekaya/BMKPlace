using System;
using System.Collections.Concurrent;
using SRWorkerServer.GridCache.Data.Entities;

namespace SRWorkerServer.GridCache.Services;

public class GridDictionary
{
    private readonly ConcurrentDictionary<string, GridEntity> _internalDictionary;

    public GridDictionary(){
        _internalDictionary = new ConcurrentDictionary<string, GridEntity>();
    }

    public bool TryAdd(string key, GridEntity gridEntity)
    {
        return _internalDictionary.TryAdd(key, gridEntity);
    }

    public bool TryGetValue(string key, out GridEntity? gridEntity)
    {
        return _internalDictionary.TryGetValue(key, out gridEntity);
    }

    // Custom AddPoint method that ensures thread safety
    public bool TryAddPoint(string key, int posX, int posY, byte value)
    {
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

    public byte TryGetPoint(string key, int posX, int posY){
        if(_internalDictionary.TryGetValue(key, out var entity)){
            int pos = entity.CalculatePosition(posX, posY);
            bool isInBounds = pos >= 0 && pos < entity.Bytes.Length;

            if (!isInBounds){
                return 0;
            }

            return entity.Bytes[pos];
        }
        return 0;
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

    public int Count => _internalDictionary.Count;
}
