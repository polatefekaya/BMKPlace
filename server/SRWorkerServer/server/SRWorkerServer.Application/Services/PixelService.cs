using System;
using Microsoft.Extensions.Logging;
using SRWorkerServer.Application.Interfaces;
using SRWorkerServer.Application.Interfaces.Infrastructure;
using SRWorkerServer.Domain.Data.DTO;
using SRWorkerServer.Domain.Data.DTO.Request;

namespace SRWorkerServer.Application.Services;

public class PixelService : IPixelService
{
    private readonly ILogger<PixelService> _logger;
    private readonly IPixelMessageBrokerService _messageBroker;
    private readonly IPixelRealtimeService _realtime;
    private readonly ICorePixelService _coreService;
    private readonly string _className;

    public PixelService(ILogger<PixelService> logger, IPixelMessageBrokerService messageBroker, IPixelRealtimeService realtime, ICorePixelService coreService){
        _logger = logger;
        _realtime = realtime;
        _messageBroker = messageBroker;
        _coreService = coreService;

        _className = nameof(PixelService);
    }

    public async Task AddPixelAsync(string groupName, PixelAddDto dto)
    {
        await _coreService.PublishAndBroadcast<PixelAddDto, PixelAddDto>(groupName, "c", dto, nameof(AddPixelAsync), _className);
    }

    public async Task GetAllPixelsByCanvas(string groupName, PixelGetAllByCanvasRequestDto dto)
    {
        await _coreService.PublishAndBroadcast<PixelGetAllByCanvasRequestDto, PixelGetAllByCanvasRequestDto>(groupName, "c", dto, nameof(GetAllPixelsByCanvas), _className);
    }

    public async Task GetByPosition(string groupName, PixelGetByPositionDto dto)
    {
        await _coreService.PublishAndBroadcast<PixelGetByPositionDto, PixelGetByPositionDto>(groupName, "c", dto, nameof(GetByPosition), _className);
    }

    public Task OnConnectedAsync(string groupName, string message)
    {
        throw new NotImplementedException();
    }

    public Task OnConnectionCloseAsync(string groupName, object message)
    {
        throw new NotImplementedException();
    }

    public Task OnFirstConnectAsync(string groupName, FirstConnectDto dto)
    {
        throw new NotImplementedException();
    }

    public Task SendPixelUpdate(string groupName, SendPixelUpdateDto dto)
    {
        throw new NotImplementedException();
    }
}
