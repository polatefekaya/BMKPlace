using System;
using SRWorkerServer.Domain.Data.DTO;
using SRWorkerServer.Domain.Data.DTO.Request;

namespace SRWorkerServer.Application.Interfaces;

public interface IPixelService
{
    public Task AddPixelAsync(string groupName, PixelAddDto dto);
    public Task GetAllPixelsByCanvas(string groupName, PixelGetAllByCanvasRequestDto rdto);
    public Task GetByPosition(string groupName, PixelGetByPositionDto dto);
    public Task OnFirstConnectAsync(string groupName, FirstConnectDto dto);
    public Task OnConnectedAsync(string groupName, ConnectedDto dto);
    public Task OnConnectionCloseAsync(string groupName, ConnectionCloseDto dto);
    public Task SendPixelUpdate(string groupName, SendPixelUpdateDto dto);
}
