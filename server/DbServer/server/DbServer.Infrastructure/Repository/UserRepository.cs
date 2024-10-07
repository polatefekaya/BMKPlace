using System;
using Dapper;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;

namespace DbServer.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public UserRepository(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }
    public Task<UserEntity> Add()
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity> Delete()
    {
        throw new NotImplementedException();
    }

    public Task<UserEntity[]> DeleteMany()
    {
        throw new NotImplementedException();
    }

    public async Task<UserEntity> Get()
    {
        UserEntity? entity = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<UserEntity>(
            "SELECT * FROM users WHEN @id == users.id",
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
        throw new NotImplementedException();
    }
}
