using System;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Services.Database.SqlQuery;

public class UserQuerySevice : IUserQueryService
{
    public string UpdateById => "UPDATE users SET IpAddress = @IpAdress, Username = @Username, Email = @Email, PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt, CreatedAt = @CreatedAt, UpdatedAt = @UpdatedAt WHERE Id = @Id;";

    public string GetById => "SELECT * FROM users WHERE Id = @Id;";

    public string DeleteById => "DELETE FROM users WHERE Id = @Id;";

    public string Add => "INSERT INTO users (IpAddress, Username, Email, PasswordHash, PasswordSalt, CreatedAt, UpdatedAt) VALUES (@IpAddress, @Username, @Email, @PasswordHash, @PasswordSalt, @CreatedAt, @UpdatedAt);";
}