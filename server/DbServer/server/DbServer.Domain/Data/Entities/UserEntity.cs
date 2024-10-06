using System;
using System.ComponentModel.DataAnnotations;

namespace DbServer.Domain.Data.Entities;

public class UserEntity
{
    [Key]
    public int Id {get; set;}
    public string? IpAddress {get; set;} = null;
    public string? Username {get; set;} = null;
    public string? Email {get; set;} = null;
    public string? PasswordHash {get; set;} = null;
    public string? PasswordSalt {get; set;} = null;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}
