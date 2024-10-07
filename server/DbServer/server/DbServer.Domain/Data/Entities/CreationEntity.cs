using System;
using System.ComponentModel.DataAnnotations;

namespace DbServer.Domain.Data.Entities;

public class CreationEntity
{
    [Key]
    public int Id {get; set;}
    public int CanvasId {get; set;}
    public int UserId {get; set;}
    public DateTime CreatedAt {get; set;}
}
