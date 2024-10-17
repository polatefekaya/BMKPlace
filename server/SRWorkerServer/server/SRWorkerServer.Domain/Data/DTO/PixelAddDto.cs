using System;

namespace SRWorkerServer.Domain.Data.DTO;

public class PixelAddDto
{
    public int UserId {get; set;}
    public int CanvasId {get; set;}
    public int ColorId {get; set;}
    public string? PosX {get; set;}
    public string? PosY {get; set;}
}
