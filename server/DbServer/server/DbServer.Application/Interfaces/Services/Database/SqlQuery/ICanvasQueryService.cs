using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Services.Database.SqlQuery;

public interface ICanvasQueryService : ISqlQueryService<CanvasEntity>
{
    public string UpdateById { get; }
}
