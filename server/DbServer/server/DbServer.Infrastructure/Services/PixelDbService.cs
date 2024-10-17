using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Models.Errors;
using DbServer.Domain.Data.Results;
using Microsoft.Extensions.Logging;

namespace DbServer.Infrastructure.Services;

public class PixelDbService : IPixelDbService
{
    private readonly IPixelRepository _repository;
    private readonly IDbServiceHelper _helper;
    private readonly string _className;
    public PixelDbService(IPixelRepository pixelRepository, IDbServiceHelper helper){
        _repository = pixelRepository;

        _helper = helper;
        _className = this.GetType().Name;
    }

    public async Task<DatabaseResult<PixelEntity>> Add(PixelEntity entity)
    {
        string methodName = nameof(Add);

        DatabaseResult<PixelEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Add(entity);
        }, methodName, _className);

        return result;        
    }

    public async Task<DatabaseResult<PixelEntity>> Delete(int id)
    {
        string methodName = nameof(Delete);

        DatabaseResult<PixelEntity> result = await _helper.RunWithTransaction(async () => {
            return await _repository.Delete(id);
        }, methodName, _className);

        return result;    
    }

    public async Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByColor(string color)
    {
        string methodName = nameof(DeleteManyByColor);

        DatabaseResult<IEnumerable<PixelEntity>> result = await _helper.RunWithTransaction(async () => {
            return await _repository.DeleteManyByColor(color);
        }, methodName, _className);

        return result;    
    }

    public async Task<DatabaseResult<IEnumerable<PixelEntity>>> DeleteManyByDate(string date)
    {
        string methodName = nameof(DeleteManyByDate);

        DatabaseResult<IEnumerable<PixelEntity>> result = await _helper.RunWithTransaction(async () => {
            return await _repository.DeleteManyByDate(date);
        }, methodName, _className);

        return result;    
    }

    public async Task<DatabaseResult<PixelEntity>> Get(int id)
    {
        string methodName = nameof(Get);

        DatabaseResult<PixelEntity> result = await _helper.Run(async () => {
            return await _repository.Get(id);
        }, methodName, _className);

        return result;    
    }

    public async Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByColor(string color)
    {
        string methodName = nameof(GetManyByColor);

        DatabaseResult<IEnumerable<PixelEntity>> result = await _helper.Run(async () => {
            return await _repository.GetManyByColor(color);
        }, methodName, _className);

        return result;    
    }

    public async Task<DatabaseResult<IEnumerable<PixelEntity>>> GetManyByDate(string date)
    {
        string methodName = nameof(GetManyByDate);

        DatabaseResult<IEnumerable<PixelEntity>> result = await _helper.Run(async () => {
            return await _repository.GetManyByDate(date);
        }, methodName, _className);

        return result;  
    }
}
