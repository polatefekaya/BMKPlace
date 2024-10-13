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


    public Task AddPixelAsync(PixelAddDto dto)
    {
        throw new NotImplementedException();
    }

    public Task GetAllPixelsByCanvas(PixelGetAllByCanvasRequestDto rdto)
    {
        throw new NotImplementedException();
    }

    public Task GetByPosition(PixelGetByPositionDto dto)
    {
        throw new NotImplementedException();
    }
}
