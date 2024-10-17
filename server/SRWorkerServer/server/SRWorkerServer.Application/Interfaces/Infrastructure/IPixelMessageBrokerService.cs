using System;
using System.Threading.Channels;

namespace SRWorkerServer.Application.Interfaces.Infrastructure;

public interface IPixelMessageBrokerService
{
    public void PublishMessage<T, TMessage>(TMessage message, string queue);
    public void PublishRPCMessage<T, TMessage>(TMessage message, string queue, Channel<byte[]> channel, bool channelPerCall = false);
}
