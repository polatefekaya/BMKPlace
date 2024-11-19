using System;
using SRWorkerServer.GridCache.Interfaces;

namespace SRWorkerServer.GridCache.Services;

public class BitwiseService : IBitwiseService
{
    public Task<byte> Extract(byte number, byte part)
    {
        throw new NotImplementedException();
    }

    public Task<byte> Merge(byte first, byte second, byte slice)
    {
        throw new NotImplementedException();
    }

    public Task<byte> Shift(byte number, byte amount)
    {
        throw new NotImplementedException();
    }
}
