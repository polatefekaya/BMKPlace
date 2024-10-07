using System;
using System.ComponentModel.DataAnnotations;

namespace DbServer.Domain.Data.Entities;

public class PixelEntity
{
    [Key]
    public int Id {get; set;}
    public int ColorId {get; set;}
    public int PosX {get; set;}
    public int PosY {get; set;}
    public DateTime CreatedAt {get; set;}

}
