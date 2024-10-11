using System;

namespace RestServer.Domain.Data.Entities;

public class UserEntity
{
    public int Id {get; set;}
    public string? IpAddress {get; set;}
    public string? Username {get; set;}
    public string? Email {get; set;}
    public string? PasswordHash {get; set;}
    public string? PasswordSalt {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
}
