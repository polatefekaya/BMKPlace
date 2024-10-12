using System;
using MessagePack;

namespace RestServer.Domain.Data.Entities;

[MessagePackObject]
public class UserEntity
{
    [Key(0)]
    public int Id {get; set;}
    [Key(1)]
    public string? IpAddress {get; set;}
    [Key(2)]
    public string? Username {get; set;}
    [Key(3)]
    public string? Email {get; set;}
    [Key(4)]
    public string? PasswordHash {get; set;}
    [Key(5)]
    public string? PasswordSalt {get; set;}
    [Key(6)]
    public DateTime CreatedAt {get; set;}
    [Key(7)]
    public DateTime UpdatedAt {get; set;}
}
