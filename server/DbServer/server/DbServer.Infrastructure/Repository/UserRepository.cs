using System;
using Dapper;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Models.Errors;
using DbServer.Domain.Data.Results;
using Microsoft.Extensions.Logging;

namespace DbServer.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserQueryService _userQueryService;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory, ILogger<UserRepository> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;

        _userQueryService = sqlServiceFactory.Create().UserQueryService();
    }
    public async Task<DatabaseResult<UserEntity>> Add(UserEntity entity)
    {
        _unitOfWork.Begin();
        _logger.LogDebug("User Add Transaction Began");

        string sql = _userQueryService.Add;
        int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
            sql,
            entity,
            _unitOfWork.Transaction
        );
        
        if (rowsAffected < 1){
            _logger.LogWarning("No rows affected, Addition can not be made");

            _unitOfWork.Rollback();
            _logger.LogDebug("User Add Transaction Rolledback");

            return DatabaseResult<UserEntity>.Error("No rows affected, Addition can not be made", ErrorTypes.CanNotMade, rowsAffected);
        }

        _unitOfWork.Commit();
        _logger.LogDebug("User Add Transaction Committed");

        _logger.LogInformation("User Additon is succesful");
        return DatabaseResult<UserEntity>.Success();
    }

    public async Task<DatabaseResult<UserEntity>> Delete(int id)
    {
        _unitOfWork.Begin();
        _logger.LogDebug("User Delete Transaction Began");

        string sql = _userQueryService.DeleteById;
        int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
            sql,
            new { Id = id },
            _unitOfWork.Transaction
        );

        if (rowsAffected < 1){
            _logger.LogWarning("No rows affected, Deletion can not be made");

            _unitOfWork.Rollback();
            _logger.LogDebug("User Delete Transaction Rolledback");

            return DatabaseResult<UserEntity>.Error("No rows affected, Deletion can not be made", ErrorTypes.CanNotMade, rowsAffected);
        }

        _unitOfWork.Commit();
        _logger.LogDebug("User Delete Transaction Committed");

        _logger.LogInformation("User Deletion is successful");
        return DatabaseResult<UserEntity>.Success();
    }

    public Task<DatabaseResult<IEnumerable<UserEntity>>> DeleteMany()
    {

        throw new NotImplementedException();
    }

    public async Task<DatabaseResult<UserEntity>> Get(int id)
    {
        _unitOfWork.Begin();
        _logger.LogDebug("User Get Transaction Began");

        string sql = _userQueryService.GetById;
        UserEntity? entity = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<UserEntity>(
            sql,
            new { Id = id },
            _unitOfWork.Transaction);

        if (entity is null)
        {
            _logger.LogWarning("User Entity not found");

            _unitOfWork.Rollback();
            _logger.LogDebug("User Get Transaction Rolledback");
            
            return DatabaseResult<UserEntity>.Error("User Entity not found", ErrorTypes.NotFound);
        }

        _unitOfWork.Commit();
        _logger.LogDebug("User Get Transaction Committed");

        _logger.LogInformation("User Entity found");
        return DatabaseResult<UserEntity>.Success(entity);
    }

    public Task<DatabaseResult<IEnumerable<UserEntity>>> GetMany()
    {
        throw new NotImplementedException();
    }

    public async Task<DatabaseResult<UserEntity>> Update(UserEntity entity)
    {
        _unitOfWork.Begin();
        _logger.LogDebug("User Update Transaction Began");

        string sql = _userQueryService.UpdateById;

        int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
            sql,
            entity,
            _unitOfWork.Transaction
        );

        if (rowsAffected < 1){
            _logger.LogWarning("No rows affected, Updates can not be made");

            _unitOfWork.Rollback();
            _logger.LogDebug("User Update Transaction Rolledback");

            return DatabaseResult<UserEntity>.Error("No rows affected, Updates can not be made", ErrorTypes.CanNotMade, rowsAffected);
        }

        _unitOfWork.Commit();
        _logger.LogDebug("User Update Transaction Committed");

        _logger.LogInformation("User Update made successfully");
        return DatabaseResult<UserEntity>.Success();
    }
}
