using System;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;

namespace DbServer.Application.Services.Database.SqlQuery;

public class CanvasQueryService : ICanvasQueryService
{
    public string UpdateById => "UPDATE canvases SET SizeX = @SizeX, SizeY = @SizeY, Name = @Name, PixelEntryCount = @PixelEntryCount, UserContributedCount = @UserContributedCount, CreatedAt = @CreatedAt, UpdatedAt = @UpdatedAt WHERE Id = @Id";

    public string GetById => "SELECT * FROM canvases WHERE Id = @Id;";

    public string DeleteById => "DELETE FROM canvases WHERE Id = @Id";

    public string Add => "INSERT INTO canvases (SizeX, SizeY, Name, PixelEntryCount, UserContributedCount, CreatedAt, UpdatedAt) VALUES (@SizeX, @SizeY, @Name, @PixelEntryCount, @UserContributedCount, @CreatedAt, @UpdatedAt)";
}
