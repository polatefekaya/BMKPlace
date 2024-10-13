using System;
using SRWorkerServer.Domain.Data.Entities;

namespace SRWorkerServer.Domain.Data.DTO.Response;

public class PixelGetAllByCanvasResponseDto
{
    public PixelEntity[] Pixels {get; set;}
}
