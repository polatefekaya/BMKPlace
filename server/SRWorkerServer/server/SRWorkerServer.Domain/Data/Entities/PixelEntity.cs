using System;
using MessagePack;

namespace SRWorkerServer.Domain.Data.Entities;

[MessagePackObject]
public class PixelEntity
{
    [Key(0)]
    public int Id {get; set;}
    [Key(1)]
    public int ColorId {get; set;}
    [Key(2)]
    public int PosX {get; set;}
    [Key(3)]
    public int PosY {get; set;}
    [Key(4)]
    public DateTime CreatedAt {get; set;}

}
