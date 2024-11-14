using System;
using SRWorkerServer.Domain.Data.Entities;

namespace SRWorkerServer.Domain.Data.DTO.Response;

public record class PixelGetAllByCanvasResponseDto
{
    public required PixelEntity[] Pixels {get; init;}
}
