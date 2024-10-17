using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Infrastructure.Services;

public class CreatorDbService : ICreatorDbService
{
    private readonly ICreatorRepository _repository;
    private readonly IDbServiceHelper _helper;
    private readonly string _className;

    public CreatorDbService(ICreatorRepository repository, IDbServiceHelper helper){
        _repository = repository;
        _helper = helper;

        _className = this.GetType().Name;
    }
    public async Task<DatabaseResult<CreationEntity>> Add(CreationEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<CreationEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Add(entity);
        }, methodName, _className);

        return result; 
    }

    public async Task<DatabaseResult<CreationEntity>> Delete(int id)
    {
        string methodName = nameof(Delete);

        DatabaseResult<CreationEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Delete(id);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<CreationEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<CreationEntity> result = await _helper.Run(async () => {
            return await _repository.Get(id);
        }, methodName, _className);

        return result;
    }
}
