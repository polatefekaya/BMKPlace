using System;

namespace SRWorkerServer.Domain.Data.DTO.Request;

public record class PixelGetAllByCanvasRequestDto
{
    public int CanvasId {get; init;}
}
