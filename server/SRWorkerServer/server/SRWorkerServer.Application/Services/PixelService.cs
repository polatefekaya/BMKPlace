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

    public PixelService(ILogger<PixelService> logger, IPixelMessageBrokerService messageBroker, IPixelRealtimeService realtime){
        _logger = logger;
        _realtime = realtime;
        _messageBroker = messageBroker;
    }


    public async Task AddPixelAsync(PixelAddDto dto)
    {
        _logger.LogDebug("Pixel Adding is started.");
        _messageBroker.PublishMessage<PixelAddDto, PixelAddDto>(dto, "c");
        await _realtime.BroadcastMessage<PixelAddDto>(dto);
    }

    public async Task GetAllPixelsByCanvas(PixelGetAllByCanvasRequestDto dto)
    {
        _logger.LogDebug("Pixel GetAllPixelsByCanvas is started.");
        _messageBroker.PublishMessage<PixelGetAllByCanvasRequestDto, PixelGetAllByCanvasRequestDto>(dto, "c");
        await _realtime.BroadcastMessage<PixelGetAllByCanvasRequestDto>(dto);
    }

    public async Task GetByPosition(PixelGetByPositionDto dto)
    {
        _logger.LogDebug("Pixel GetByPosition is started.");
        _messageBroker.PublishMessage<PixelGetByPositionDto, PixelGetByPositionDto>(dto, "c");
        await _realtime.BroadcastMessage<PixelGetByPositionDto>(dto);
    }
}
