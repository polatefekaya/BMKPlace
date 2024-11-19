using System;

namespace SRWorkerServer.Domain.Data.DTO;

public record class SendPixelUpdateDto
{
    public int x {get; init;}
    public int y {get; init;}
    public int colorIndex {get; init;}
}
