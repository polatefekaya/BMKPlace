using System;

namespace SRWorkerServer.Domain.Data.DTO;

public record class PixelGetByPositionDto
{
    public int CanvasId {get; init;}
    public required string PosX {get; init;}
    public required string PosY {get; init;}
}
