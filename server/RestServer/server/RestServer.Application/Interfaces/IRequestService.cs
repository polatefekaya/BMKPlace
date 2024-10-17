using System;
using System.Threading.Channels;

namespace RestServer.Application.Interfaces;

public interface IRequestService
{
    public void Publish<T, TMessage>(TMessage entity, string queue);
    public Task<T> PublishRPC<T, TMessage>(TMessage entity, string queue, bool channelPerCall);
}
