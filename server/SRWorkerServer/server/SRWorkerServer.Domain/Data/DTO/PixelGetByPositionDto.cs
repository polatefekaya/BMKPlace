using System;

namespace SRWorkerServer.Domain.Data.DTO;

public class PixelGetByPositionDto
{
    public int CanvasId {get; set;}
    public string? PosX {get; set;}
    public string? PosY {get; set;}
}
