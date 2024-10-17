using System;
using DbServer.Domain.Data.Entities;
using DbServer.Domain.Data.Results;

namespace DbServer.Application.Interfaces.Repository;

public interface IContributionRepository
{
    public Task<DatabaseResult<ContributionEntity>> Get(int id);
    public Task<DatabaseResult<ContributionEntity>> Add(ContributionEntity entity);
    public Task<DatabaseResult<ContributionEntity>> Delete(int id);
    public Task<DatabaseResult<ContributionEntity>> GetByPositionWithCanvasId(string position, int canvasId);
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByUserId(int userId);
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByCanvasId(int canvasId);
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> GetManyByPixelId(int pixelId);
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByUserId(int userId);
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByCanvasId(int canvasId);
    public Task<DatabaseResult<IEnumerable<ContributionEntity>>> DeleteManyByPixelId(int pixelId);
    public Task<DatabaseResult<ContributionEntity>> DeleteByPositionWithCanvasId(string position, int canvasId);
}

