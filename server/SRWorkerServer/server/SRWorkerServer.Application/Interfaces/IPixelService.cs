using System;
using SRWorkerServer.Domain.Data.DTO;
using SRWorkerServer.Domain.Data.DTO.Request;

namespace SRWorkerServer.Application.Interfaces;

public interface IPixelService
{
    public Task AddPixelAsync(PixelAddDto dto);
    public Task GetAllPixelsByCanvas(PixelGetAllByCanvasRequestDto rdto);
    public Task GetByPosition(PixelGetByPositionDto dto);
}
