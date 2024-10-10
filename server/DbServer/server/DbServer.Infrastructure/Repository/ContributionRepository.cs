using System;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Infrastructure.Repository;

public class ContributionRepository : IContributionRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IContributionQueryService _contributionQueryService;

    public ContributionRepository(IUnitOfWork unitOfWork, ISqlQueryServiceFactory sqlServiceFactory)
    {
        _unitOfWork = unitOfWork;
        _contributionQueryService = sqlServiceFactory.Create().ContributionQueryService();
    }
    public Task<DatabaseResult<ContributionEntity>> Add()
    {
        string sql = _contributionQueryService.Add;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<ContributionEntity>> Delete()
    {
        string sql = _contributionQueryService.DeleteById;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteMany()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByCanvasId()
    {
        string sql = _contributionQueryService.DeleteManyByCanvasId;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByPixelId()
    {
        string sql = _contributionQueryService.DeleteManyByPixelId;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByUserId()
    {
        string sql = _contributionQueryService.DeleteManyByUserId;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<ContributionEntity>> Get()
    {
        string sql = _contributionQueryService.GetById;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<ContributionEntity>> GetByPosition()
    {
        string sql = _contributionQueryService.GetByPositionWithCanvasId;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetMany()
    {
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByCanvasId()
    {
        string sql = _contributionQueryService.GetManyByCanvasId;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByPixelId()
    {
        string sql = _contributionQueryService.GetManyByPixelId;
        throw new NotImplementedException();
    }

    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByUserId()
    {
        string sql = _contributionQueryService.GetManyByUserId;
        throw new NotImplementedException();
    }
}
