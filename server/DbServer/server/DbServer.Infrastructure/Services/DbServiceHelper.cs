using System;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Models.Errors;
using DbServer.Domain.Data.Results;
using Microsoft.Extensions.Logging;

namespace DbServer.Infrastructure.Services;

public class DbServiceHelper : IDbServiceHelper
{
    private readonly ILogger<DbServiceHelper> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public DbServiceHelper(ILogger<DbServiceHelper> logger, IUnitOfWork unitOfWork){
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<DatabaseResult<T>> Run<T>(Func<Task<DatabaseResult<T>>> operation, string methodName, string className)
    {
        _logger.LogDebug("{className} {methodName} starting", methodName, className);

        try{
            DatabaseResult<T> result = await operation.Invoke();

            if (result.IsError){
                _logger.LogTrace("Error code: {errorCode}\nError message: {errorMessage}\nError timestamp: {timestamp}\nAffected row count: {affectedRows}", result.Model.Code, result.Model.ErrorMessage, result.Timestamp, result.RowsAffected);
                return result;
            } 

            _logger.LogInformation("{className} {methodName} is successful", methodName, className);
            _logger.LogTrace("Rows Affected: {rowsAffected}", result.RowsAffected);
            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "An unexpected error occurred while {className} {methodName}", methodName, className);

            return DatabaseResult<T>.Error($"An unexpected error occurred while {className} {methodName}", ErrorTypes.Unexpected);
        }
    }

    public async Task<DatabaseResult<T>> RunWithTransaction<T>(Func<Task<DatabaseResult<T>>> operation, string methodName, string className)
    {
        _logger.LogDebug("{className} {methodName} transaction starting", methodName, className);
        _unitOfWork.Begin();

        try{
            DatabaseResult<T> result = await operation.Invoke();

            if (result.IsError){
                _logger.LogWarning("{className} {methodName} transaction failed, rolling back", methodName, className);
                _unitOfWork.Rollback();

                _logger.LogTrace("Error code: {errorCode}\nError message: {errorMessage}\nError timestamp: {timestamp}\nAffected row count: {affectedRows}", result.Model.Code, result.Model.ErrorMessage, result.Timestamp, result.RowsAffected);
                return result;
            } 

            _logger.LogDebug("{className} {methodName} transaction committing", methodName, className);
            _unitOfWork.Commit();

            _logger.LogInformation("{className} {methodName} is successful", methodName, className);
            _logger.LogTrace("Rows Affected: {rowsAffected}", result.RowsAffected);
            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "An unexpected error occurred while {className} {methodName}, rolling back", methodName, className);
            _unitOfWork.Rollback();

            return DatabaseResult<T>.Error($"An unexpected error occurred while {className} {methodName}", ErrorTypes.Unexpected);
        }
    }
}
