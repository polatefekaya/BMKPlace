using System;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;

namespace DbServer.Application.Interfaces.Factories.Database;

public interface ISqlQueryServiceFactory
{
    public IQueryServiceFactory Create();

    interface IQueryServiceFactory {
        public IUserQueryService UserQueryService();
        public ICanvasQueryService CanvasQueryService();
        public ICreationQueryService CreationQueryService();
        public IContributionQueryService ContributionQueryService();
        public IPixelQueryService PixelQueryService();
    }
}
