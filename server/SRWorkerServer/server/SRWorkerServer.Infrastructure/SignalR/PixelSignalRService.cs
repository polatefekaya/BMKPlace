using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SRWorkerServer.Application.Interfaces.Infrastructure;
using SRWorkerServer.Infrastructure.SignalR.Hubs;

namespace SRWorkerServer.Infrastructure.SignalR;

public class PixelSignalRService : IPixelRealtimeService
{
    private readonly ILogger<PixelSignalRService> _logger;
    private readonly IHubContext<PixelHub> _hubContext;

    public PixelSignalRService(ILogger<PixelSignalRService> logger, IHubContext<PixelHub> hubContext){
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task BroadcastMessage<T>(string groupName, T message)
    {
        _logger.LogInformation("Broadcast Message started");
        bool isGroupNameEmpty = string.Equals(groupName, string.Empty);

        if(isGroupNameEmpty){
            _logger.LogInformation("Group name is empty, broadcasting to all users");

            await _hubContext.Clients.All.SendAsync("methodName", message);
            return;
        }

        _logger.LogInformation("Broadcasting to {groupName}'s users", groupName);

        await _hubContext.Clients.Group(groupName).SendAsync("methodName", message);
        _logger.LogDebug("Broadcast Message finished");
    }
}
