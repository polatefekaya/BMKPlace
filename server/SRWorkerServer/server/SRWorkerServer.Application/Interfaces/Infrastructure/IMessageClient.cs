using System;
using System.Threading.Channels;

namespace SRWorkerServer.Application.Interfaces.Infrastructure;

public interface IMessageClient
{
    public void SendMessage<T, TMessage>(TMessage message, string qName);
    public void SendRPCMessage<T, TMessage>(TMessage message, string qName, Channel<byte[]> channel, bool channelPerCall = false);
}
