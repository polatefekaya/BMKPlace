using System;
using SRWorkerServer.Application.Interfaces.Infrastructure;

namespace SRWorkerServer.Infrastructure.SignalR;

public class PixelSignalRService : IPixelRealtimeService
{
    public Task BroadcastMessage<T>(T message)
    {
        throw new NotImplementedException();
    }
}
