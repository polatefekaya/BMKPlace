using System;
using System.ComponentModel.DataAnnotations;

namespace DbServer.Domain.Data.Entities;

public class ColorEntity
{
    [Key]
    public int Id {get; set;}
    public string? Name {get; set;} = null;
    public string? Color {get; set;} = null;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}
