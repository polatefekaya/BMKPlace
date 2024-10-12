using System;
using System.Threading.Channels;

namespace RestServer.Application.Interfaces;

public interface IChannelManagerService
{
    public Channel<byte[]>? Get(string key = "", bool createNew = false);
}
