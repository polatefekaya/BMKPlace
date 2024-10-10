using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Models.Errors;
using DbServer.Domain.Data.Results;
using Microsoft.Extensions.Logging;

namespace DbServer.Infrastructure.Services;

public class UserDbService : IUserDbService
{
    private readonly IUserRepository _repository;
    private readonly IDbServiceHelper _helper;
    private readonly string _className;
    public UserDbService(IUserRepository userRepository, IDbServiceHelper helper){
        _repository = userRepository;

        _helper = helper;
        _className = this.GetType().Name;
    }

    public async Task<DatabaseResult<UserEntity>> Add(UserEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<UserEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Add(entity);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<UserEntity>> Delete(int id)
    {
        string methodName = nameof(Delete);

        DatabaseResult<UserEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Delete(id);
        }, methodName, _className);
        
        return result;
    }

    public async Task<DatabaseResult<UserEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<UserEntity> result = await _helper.Run(async () => {
            return await _repository.Get(id);
        }, methodName, _className);
        
        return result;
    }

    public async Task<DatabaseResult<UserEntity>> Update(UserEntity entity)
    {
        string methodName = nameof(Update);

        DatabaseResult<UserEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Update(entity);
        }, methodName, _className);
        
        return result;
        
    }
}
