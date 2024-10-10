using System;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface IRepositoryHelper
{
    public Task<DatabaseResult<T>> QueryAsync<T>(Func<Task<T?>> operation, string methodName, string className);
    public Task<DatabaseResult<T>> ExecuteAsync<T>(Func<Task<int>> operation, string methodName, string className);
}
