using System;
using Microsoft.AspNetCore.SignalR;
using SRWorkerServer.Application.Interfaces;
using SRWorkerServer.Domain.Data.DTO;
using SRWorkerServer.Domain.Data.DTO.Request;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace SRWorkerServer.Infrastructure.SignalR.Hubs;

public class PixelHub : Hub
{
    private readonly ILogger<PixelHub> _logger;
    private readonly IPixelService _pixelService;

    public PixelHub(ILogger<PixelHub> logger, IPixelService pixelService){
        _logger = logger;
        _pixelService = pixelService;
    }

    public async Task OnFirstConnectAsync(FirstConnectDto message){
        _logger.LogInformation("First Connection");
    }

    public async Task OnConnectedAsync(string message){
        _logger.LogInformation("connected" + message);
    }

    public async Task OnConnectionCloseAsync(object message){

    }

    public async Task SendPixelUpdate(object message){
        SendPixelUpdateDto? dto = JsonConvert.DeserializeObject<SendPixelUpdateDto>(message.ToString());

        _logger.LogInformation("Send Pixel Update\nx: {x}, y: {y}, colorIndex: {colorIndex}", dto?.x, dto?.y, dto?.colorIndex);
    }

    // public async Task AddPixel(PixelAddDto dto){
    //     await _pixelService.AddPixelAsync(dto);
    // }

    // public async Task GetPixelByPosition(PixelGetByPositionDto dto){
    //     await _pixelService.GetByPosition(dto);
    // }

    // //client calls this when first login
    // public async Task GetPixelsByCanvas(PixelGetAllByCanvasRequestDto dto){
    //     await _pixelService.GetAllPixelsByCanvas(dto);
    // }

    
}
