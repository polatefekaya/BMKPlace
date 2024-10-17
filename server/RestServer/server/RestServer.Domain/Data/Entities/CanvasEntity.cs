using System;
using MessagePack;

namespace RestServer.Domain.Data.Entities;

[MessagePackObject]
public class CanvasEntity
{
    [Key(0)]
    public int Id {get; set;}
    [Key(1)]
    public int SizeX {get; set;}
    [Key(2)]
    public int SizeY {get; set;}
    [Key(3)]
    public string? Name {get; set;}
    [Key(4)]
    public int PixelCount {get; set;}
    [Key(5)]
    public int PixelEntryCount {get; set;}
    [Key(6)]
    public int UserContributedCount {get; set;}
    [Key(7)]
    public DateTime CreatedAt {get; set;}
    [Key(8)]
    public DateTime UpdatedAt {get; set;}
}
