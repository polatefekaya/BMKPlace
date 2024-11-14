using System;

namespace SRWorkerServer.Domain.Data.DTO;

public record class PixelAddDto
{
    public int UserId {get; init;}
    public int CanvasId {get; init;}
    public int ColorId {get; init;}
    public string? PosX {get; init;}
    public string? PosY {get; init;}
}
