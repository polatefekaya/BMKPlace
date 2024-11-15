using System;

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
    public Task<byte> GetByte(string gridName, int posX, int posY);

    /// <summary>
    /// It resets value to 0 (white color)
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    public Task ResetByte(string gridName, int posX, int posY);
}
