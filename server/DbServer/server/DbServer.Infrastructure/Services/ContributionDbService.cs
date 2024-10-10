using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Infrastructure.Services;

public class ContributionDbService : IContributionDbService
{
    private readonly IDbServiceHelper _helper;
    private readonly IContributionRepository _repository;
    private readonly string _className;

    public ContributionDbService(IDbServiceHelper helper, IContributionRepository repository){
        _repository = repository;
        _helper = helper;

        _className = this.GetType().Name;
    }
    public async Task<DatabaseResult<ContributionEntity>> Add(ContributionEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<ContributionEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Add(entity);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<ContributionEntity>> Delete(int id)
    {
        string methodName = nameof(Delete);

        DatabaseResult<ContributionEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Delete(id);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<ContributionEntity>> DeleteByPositionWithCanvasId(string position, int canvasId)
    {
        string methodName = nameof(DeleteByPositionWithCanvasId);

        DatabaseResult<ContributionEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.DeleteByPositionWithCanvasId(position, canvasId);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByCanvasId(int canvasId)
    {
        string methodName = nameof(DeleteManyByCanvasId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.RunWithTransaction(async () => {
            return await _repository.DeleteManyByCanvasId(canvasId);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByPixelId(int pixelId)
    {
        string methodName = nameof(DeleteManyByPixelId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.RunWithTransaction(async () => {
            return await _repository.DeleteManyByPixelId(pixelId);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByUserId(int userId)
    {
        string methodName = nameof(DeleteManyByUserId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.RunWithTransaction(async () => {
            return await _repository.DeleteManyByUserId(userId);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<ContributionEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<ContributionEntity> result = await _helper.Run(async () => {
            return await _repository.Get(id);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<ContributionEntity>> GetByPositionWithCanvasId(string position, int canvasId)
    {
        string methodName = nameof(GetByPositionWithCanvasId);

        DatabaseResult<ContributionEntity> result = await _helper.Run(async () => {
            return await _repository.GetByPositionWithCanvasId(position, canvasId);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByCanvasId(int canvasId)
    {
        string methodName = nameof(GetManyByCanvasId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.Run(async () => {
            return await _repository.GetManyByCanvasId(canvasId);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByPixelId(int pixelId)
    {
        string methodName = nameof(GetManyByPixelId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.Run(async () => {
            return await _repository.GetManyByPixelId(pixelId);
        }, methodName, _className);

        return result;
    }

    public async Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByUserId(int userId)
    {
        string methodName = nameof(GetManyByUserId);

        DatabaseResult<IEnumerable<ContributionEntity>> result = await _helper.Run(async () => {
            return await _repository.GetManyByUserId(userId);
        }, methodName, _className);

        return result;
    }
}
