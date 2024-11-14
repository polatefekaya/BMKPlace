using System;

namespace SRWorkerServer.GridCache.Interfaces;

public interface IBitwiseService
{
    public Task<byte> Merge(byte first, byte second, byte slice);
    public Task<byte> Extract(byte number, byte part);
    public Task<byte> Shift(byte number, byte amount);
}
