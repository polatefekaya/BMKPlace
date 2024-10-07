using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface IUserRepository
{
    public Task<UserEntity> Get();
    public Task<UserEntity> Add();
    public Task<UserEntity> Delete();
    public Task<UserEntity> Update();
    public Task<UserEntity[]> GetMany();
    public Task<UserEntity[]> DeleteMany();
}
