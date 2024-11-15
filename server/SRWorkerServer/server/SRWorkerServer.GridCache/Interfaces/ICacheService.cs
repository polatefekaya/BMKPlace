using System;
using SRWorkerServer.GridCache.Data.Entities;

namespace SRWorkerServer.GridCache.Interfaces;

public interface ICacheService
{
    /// <summary>
    /// It caches the byte with given positions. Positions are based on unshrinked grid.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    public Task CacheByte(string gridName, byte value, int posX, int posY);

    /// <summary>
    /// Gets the byte with given position. Positions are based on unshrinked grid.
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    public Task<byte?> GetByte(string gridName, int posX, int posY);

    /// <summary>
    /// Gets all the bytes in given grid name. Bytes are shrinked.
    /// </summary>
    /// <param name="gridName"></param>
    /// <returns></returns>
    public Task<byte[]?> GetAllBytes(string gridName);

    /// <summary>
    /// Gets the grid with given gridName. Inner Byte array is shrinked.
    /// </summary>
    /// <param name="gridName"></param>
    /// <returns></returns>
    public Task<GridEntity?> GetGrid(string gridName);

    /// <summary>
    /// It resets value to 0 (white color)
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    public Task ResetByte(string gridName, int posX, int posY);
}
