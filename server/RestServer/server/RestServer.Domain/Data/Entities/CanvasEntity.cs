using System;

namespace RestServer.Domain.Data.Entities;

public class CanvasEntity
{
    public int Id {get; set;}
    public int SizeX {get; set;}
    public int SizeY {get; set;}
    public string? Name {get; set;}
    public int PixelCount {get; set;}
    public int PixelEntryCount {get; set;}
    public int UserContributedCount {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}
