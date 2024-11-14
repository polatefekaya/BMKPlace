using System;
using SRWorkerServer.GridCache.Data.Options;

namespace SRWorkerServer.GridCache.Interfaces;

public interface IBitwiseEngine
{
    public Task<byte> Shrink(byte first, byte second, BitwiseShrinkOptions? options = null);
}
