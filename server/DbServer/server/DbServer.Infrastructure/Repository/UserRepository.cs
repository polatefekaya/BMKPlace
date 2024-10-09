using System;
using Dapper;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserQueryService _userQueryService;

    public UserRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory){
        _unitOfWork = unitOfWork;

        _userQueryService = sqlServiceFactory.Create().UserQueryService();
    }
    public Task<UserEntity> Add()
    {
        string sql = _userQueryService.Add;
        throw new NotImplementedException();
    }

    public async Task<UserEntity> Delete()
    {
        string sql = _userQueryService.DeleteById;
        int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
            sql,
            new {Id = 1},
            _unitOfWork.Transaction
        );
        
    }

    public Task<UserEntity[]> DeleteMany()
    {
        
        throw new NotImplementedException();
    }

    public async Task<UserEntity> Get()
    {
        string sql = _userQueryService.GetById;
        UserEntity? entity = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<UserEntity>(
            sql,
            new {Id = 1},
            _unitOfWork.Transaction);
        if(entity is null){
                
        }
        return entity;
    }

    public Task<UserEntity[]> GetMany()
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity> Update()
    {
        string sql = _userQueryService.UpdateById;
        throw new NotImplementedException();
    }
}
