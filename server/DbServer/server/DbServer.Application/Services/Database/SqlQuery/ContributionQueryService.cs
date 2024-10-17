using System;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;

namespace DbServer.Application.Services.Database.SqlQuery;

public class ContributionQueryService : IContributionQueryService
{
    public string GetManyByPosition => "SELECT * FROM contributions WHERE Position = @Position;";

    public string GetManyByUserId => "SELECT * FROM contributions WHERE UserId = @UserId;";

    public string GetManyByCanvasId => "SELECT * FROM contributions WHERE CanvasId = @CanvasId;";

    public string GetManyByPixelId => "SELECT * FROM contributions WHERE PixelId = @PixelId;";

    public string GetByPositionWithCanvasId => throw new NotImplementedException();

    public string DeleteManyByPosition => "DELETE FROM contributions WHERE Position = @Position;";

    public string DeleteManyByPixelId => "DELETE FROM contributions WHERE PixelId = @PixelId;";

    public string DeleteManyByUserId => "DELETE FROM contributions WHERE UserId = @UserId;";

    public string DeleteManyByCanvasId => "DELETE FROM contributions WHERE CanvasId = @CanvasId;";

    public string DeleteByPositionWithCanvasId => throw new NotImplementedException();

    public string GetById => "SELECT * FROM contributions WHERE Id = @Id;";

    public string DeleteById => "DELETE FROM contributions WHERE Id = @Id";

    public string Add => "INSERT INTO contributions (PixelId, UserId, CanvasId, Position, CreatedAt) VALUES (@PixelId, @UserId, @CanvasId, @Position, @CreatedAt)";
}
