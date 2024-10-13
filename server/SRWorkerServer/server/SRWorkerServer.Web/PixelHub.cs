using System;
using Microsoft.AspNetCore.SignalR;
using SRWorkerServer.Application.Interfaces;
using SRWorkerServer.Domain.Data.DTO;
using SRWorkerServer.Domain.Data.DTO.Request;

namespace SRWorkerServer.Web;

public class PixelHub : Hub
{
    private ILogger<PixelHub> _logger;
    private readonly IPixelService _pixelService;

    public PixelHub(ILogger<PixelHub> logger, IPixelService pixelService){
        _logger = logger;
        _pixelService = pixelService;
    }

    public async Task AddPixel(PixelAddDto dto){
        await _pixelService.AddPixelAsync(dto);
    }

    public async Task GetPixelByPosition(PixelGetByPositionDto dto){
        await _pixelService.GetByPosition(dto);
    }

    //client calls this when first login
    public async Task GetPixelsByCanvas(PixelGetAllByCanvasRequestDto dto){
        await _pixelService.GetAllPixelsByCanvas(dto);
    }

    
}
