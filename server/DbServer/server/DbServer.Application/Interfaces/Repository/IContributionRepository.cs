using System;
using DbServer.Domain.Data.Entities;

namespace DbServer.Application.Interfaces.Repository;

public interface IContributionRepository
{
    public ContributionEntity Get();
    public ContributionEntity Add();
    public ContributionEntity Delete();
    public ContributionEntity[] GetMany();
    public ContributionEntity[] DeleteMany();
    public ContributionEntity GetByPosition();
    public ContributionEntity[] GetManyByUserId();
    public ContributionEntity[] GetManyByCanvasId();
    public ContributionEntity[] GetManyByPixelId();
    public ContributionEntity[] DeleteManyByUserId();
    public ContributionEntity[] DeleteManyByCanvasId();
    public ContributionEntity[] DeleteManyByPixelId();
}
