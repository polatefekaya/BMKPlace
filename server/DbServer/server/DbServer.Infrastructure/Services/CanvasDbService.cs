using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Models.Errors;
using DbServer.Domain.Data.Results;
using Microsoft.Extensions.Logging;

namespace DbServer.Infrastructure.Services;

public class CanvasDbService : ICanvasDbService
{
    private readonly ICanvasRepository _repository;
    private readonly IDbServiceHelper _helper;
    private readonly string _className;

    public CanvasDbService(ICanvasRepository canvasRepository, IDbServiceHelper helper){
        _repository = canvasRepository;

        _helper = helper;
        _className = this.GetType().Name;
    }

    public async Task<DatabaseResult<CanvasEntity>> Add(CanvasEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<CanvasEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Add(entity);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<CanvasEntity>> Delete(int id)
    {
        string methodName = nameof(Delete);

        DatabaseResult<CanvasEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Delete(id);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<CanvasEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<CanvasEntity> result = await _helper.Run(async () => {
            return await _repository.Get(id);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<CanvasEntity>> Update(CanvasEntity entity)
    {
        string methodName = nameof(Update);

        DatabaseResult<CanvasEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Update(entity);
        }, methodName, _className);

        return result;
    }
}
