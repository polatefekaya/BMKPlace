using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using SRWorkerServer.Application.Interfaces;
using SRWorkerServer.Application.Interfaces.Infrastructure;

namespace SRWorkerServer.Application.Services;

public class CorePixelService : ICorePixelService
{
    private readonly ILogger<CorePixelService> _logger;
    private readonly IPixelMessageBrokerService _messageBroker;
    private readonly IPixelRealtimeService _realtime;

    private readonly string _classname;

    public CorePixelService(ILogger<CorePixelService> logger, IPixelMessageBrokerService messageBroker, IPixelRealtimeService realtime){
        _logger = logger;
        _messageBroker = messageBroker;
        _realtime = realtime;

        _classname = nameof(CorePixelService);
    }

    public async Task Broadcast<TDto>(string groupName, TDto dto, string methodName, string className)
    {
        _logger.LogInformation("{methodName} is starting in {className} - ({baseClassName})", methodName, className, _classname);
        
        await _realtime.BroadcastMessage(groupName, dto);

        _logger.LogInformation("{methodName} is finished in {className} - ({baseClassName})", methodName, className, _classname);
    }

    public async Task Publish<TIn, TOut>(string queue, TOut dto, string methodName, string className)
    {
        _logger.LogInformation("{methodName} is starting in {className} - ({baseClassName})", methodName, className, _classname);
        
        _messageBroker.PublishMessage<TIn, TOut>(dto, queue);

        _logger.LogInformation("{methodName} is finished in {className} - ({baseClassName})", methodName, className, _classname);
    }

    public async Task PublishAndBroadcast<TIn, TOut>(string groupName, string queue, TOut dto, string methodName, string className)
    {
        _logger.LogInformation("{methodName} is starting in {className} - ({baseClassName})", methodName, className, _classname);
        
        _messageBroker.PublishMessage<TIn, TOut>(dto, queue);

        await _realtime.BroadcastMessage(groupName, dto);

        _logger.LogInformation("{methodName} is finished in {className} - ({baseClassName})", methodName, className, _classname);
    }
}
