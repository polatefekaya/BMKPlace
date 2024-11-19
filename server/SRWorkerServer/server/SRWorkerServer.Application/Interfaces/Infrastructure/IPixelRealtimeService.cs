using System;

namespace SRWorkerServer.Application.Interfaces.Infrastructure;

public interface IPixelRealtimeService
{
    public Task BroadcastMessage<T>(string groupName, T message);
}
