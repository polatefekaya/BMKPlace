using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Services.Database.SqlQuery;

public interface IPixelQueryService : ISqlQueryService<PixelEntity>
{
    public string GetManyByPosition { get; }
    public string GetManyByColor { get; }
    public string GetManyByDate { get; }
    public string DeleteManyByPosition { get; }
    public string DeleteManyByColor { get; }
    public string DeleteManyByDate { get; }
    public string GetByPositionWithCanvasId { get; }
    public string DeleteByPositionWithCanvasId { get; }
}
