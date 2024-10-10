using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface IUserRepository
{
    public Task<DatabaseResult<UserEntity>> Get(int id);
    public Task<DatabaseResult<UserEntity>> Add(UserEntity entity);
    public Task<DatabaseResult<UserEntity>> Delete(int id);
    public Task<DatabaseResult<UserEntity>> Update(UserEntity entity);
    public Task<DatabaseResult<IEnumerable<UserEntity>>> GetMany();
    public Task<DatabaseResult<IEnumerable<UserEntity>>> DeleteMany();
}
