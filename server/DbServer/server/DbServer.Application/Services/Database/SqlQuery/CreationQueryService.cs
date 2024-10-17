using System;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;

namespace DbServer.Application.Services.Database.SqlQuery;

public class CreationQueryService : ICreationQueryService
{
    public string GetById => "SELECT * FROM creations WHERE Id = @Id;";

    public string DeleteById => "DELETE FROM creations WHERE Id = @Id";

    public string Add => "INSERT INTO creations (SizeX, SizeY, Name, Filled, PixelCount, UserContributed, CreatedAt, UpdatedAt) VALUES (@SizeX, @SizeY, @Name, @Filled, @PixelCount, @UserContributed, @CreatedAt, @UpdatedAt)";
}
