using System;

namespace SRWorkerServer.Domain.Data.DTO;

public class SendPixelUpdateDto
{
    public int x {get; set;}
    public int y {get; set;}
    public int colorIndex {get; set;}
}
