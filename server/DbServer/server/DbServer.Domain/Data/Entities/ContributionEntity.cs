using System;
using System.ComponentModel.DataAnnotations;

namespace DbServer.Domain.Data.Entities;

public class ContributionEntity
{
    [Key]
    public int Id {get; set;}
    public int PixelId {get; set;}
    public int UserId {get; set;}
    public int CanvasId {get; set;}
    public string Position {get; set;} = "0:0";
    public DateTime CreatedAt {get; set;}
}
