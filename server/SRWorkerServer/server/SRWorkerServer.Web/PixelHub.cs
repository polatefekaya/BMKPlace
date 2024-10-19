using System;
using Microsoft.AspNetCore.SignalR;
using SRWorkerServer.Application.Interfaces;
using SRWorkerServer.Domain.Data.DTO;
using SRWorkerServer.Domain.Data.DTO.Request;
using Newtonsoft.Json;

namespace SRWorkerServer.Web;

public class PixelHub : Hub
{
    private ILogger<PixelHub> _logger;
    //private readonly IPixelService _pixelService;

    public PixelHub(ILogger<PixelHub> logger){
        _logger = logger;
        //_pixelService = pixelService;
    }

    public async Task OnConnectedAsync(string message){
        _logger.LogInformation("connected" + message);
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
