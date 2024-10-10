using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Domain.Data.Models.Errors;
using DbServer.Domain.Data.Results;
using Microsoft.Extensions.Logging;

namespace DbServer.Infrastructure.Repository;

public class RepositoryHelper : IRepositoryHelper
{
    private readonly ILogger<RepositoryHelper> _logger;
    public RepositoryHelper(ILogger<RepositoryHelper> logger){
        _logger = logger;
    }

    public async Task<DatabaseResult<T>> ExecuteAsync<T>(Func<Task<int>> operation, string methodName, string className){
        _logger.LogDebug("{className} {methodName} Began", className, methodName);

        int rowsAffected;

        try{
            rowsAffected = await operation.Invoke();
        }
        catch (Exception ex) {   
            _logger.LogError(ex, "Exception occured while {className} {methodName}", className, methodName);
            return DatabaseResult<T>.Error($"Exception occured while {className} {methodName}", ErrorTypes.Unexpected);
        }

        if (rowsAffected < 1){
            _logger.LogWarning("No rows affected, {methodName} can not be made", methodName);

            return DatabaseResult<T>.Error($"No rows affected, {methodName} can not be made", ErrorTypes.CanNotMade, rowsAffected);
        }

        _logger.LogInformation("{className} {methodName} is succesful", className, methodName);
        return DatabaseResult<T>.Success();
    }


    public async Task<DatabaseResult<T>> QueryAsync<T>(Func<Task<T?>> operation, string methodName, string className){
         _logger.LogDebug("{className} {methodName} Began", className, methodName);

        T? result;

        try{
            result = await operation.Invoke();
        }
        catch (Exception ex) {   
            _logger.LogError(ex, "Exception occured while {className} {methodName}", className, methodName);
            return DatabaseResult<T>.Error($"Exception occured while {className} {methodName}", ErrorTypes.Unexpected);
        }

        if(result is null){
            _logger.LogWarning("Returned Result is null, {methodName} can not be made", methodName);
            return DatabaseResult<T>.Error($"Returned Result is null, {methodName} can not be made", ErrorTypes.NullResult);
        }

        _logger.LogInformation("{className} {methodName} is succesful", className, methodName);
        return DatabaseResult<T>.Success();
    }

}