using System;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Services.Database;

public interface IDbServiceHelper
{
    public Task<DatabaseResult<T>> Run<T>(Func<Task<DatabaseResult<T>>> operation, string methodName, string className);

    public Task<DatabaseResult<T>> RunWithTransaction<T>(Func<Task<DatabaseResult<T>>> operation, string methodName, string className);
}
