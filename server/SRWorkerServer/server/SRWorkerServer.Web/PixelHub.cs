using System;
using Microsoft.AspNetCore.SignalR;
using SRWorkerServer.Application.Interfaces;

namespace SRWorkerServer.Web;

public class PixelHub : Hub
{
    private ILogger<PixelHub> _logger;
    private readonly IPixelService _pixelService;

    public PixelHub(ILogger<PixelHub> logger, IPixelService pixelService){
        _logger = logger;
        _pixelService = pixelService;
    }

    public async Task AddPixel(){
        await _pixelService.AddPixelAsync();
    }

    public async Task GetPixelByPosition(){
        await _pixelService.GetByPosition();
    }

    //client calls this when first login
    public async Task GetPixelsByCanvas(){
        await _pixelService.GetAllPixelsByCanvas();
    }

    
}
