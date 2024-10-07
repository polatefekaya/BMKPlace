using System;
using System.ComponentModel.DataAnnotations;

namespace DbServer.Domain.Data.Entities;

public class CanvasEntity
{
    [Key]
    public int Id {get; set;}
    public int sizeX {get; set;}
    public int SizeY {get; set;}
    public string? Name {get; set;} = null;
    public int PixelCount {get; set;}
    public int PixelEntryCount {get; set;}
    public int UserContributedCount {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}
