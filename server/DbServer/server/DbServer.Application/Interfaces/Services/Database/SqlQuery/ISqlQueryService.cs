using System;

namespace DbServer.Application.Interfaces.Services.Database.SqlQuery;

public interface ISqlQueryService<T>
{
    public string GetById { get; }
    public string DeleteById { get; }
    public string Add { get; }
}
