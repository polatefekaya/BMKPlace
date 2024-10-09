using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Services.Database.SqlQuery;

public interface IContributionQueryService : ISqlQueryService<ContributionEntity>
{
    public string GetManyByPosition { get; }
    public string GetManyByUserId { get; }
    public string GetManyByCanvasId { get; }
    public string GetManyByPixelId { get; }
    public string GetByPositionWithCanvasId { get; }
    public string DeleteManyByPosition { get; }
    public string DeleteManyByPixelId { get; }
    public string DeleteManyByUserId { get; }
    public string DeleteManyByCanvasId { get; }
    public string DeleteByPositionWithCanvasId { get; }
}
