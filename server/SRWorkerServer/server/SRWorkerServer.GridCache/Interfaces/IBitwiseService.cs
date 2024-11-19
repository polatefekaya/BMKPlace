using System;

namespace SRWorkerServer.GridCache.Interfaces;

public interface IBitwiseService
{
    public Task<byte> Merge(byte first, byte second, byte slice);
    /// <summary>
    /// Extract the decided part of a byte, it will be mergedValue and you can extract the first or second value from it.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public Task<byte> Extract(byte number, byte part);
    public Task<byte> Shift(byte number, byte amount);
}
