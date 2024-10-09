using System;
using System.Security.Cryptography;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Services.Database.SqlQuery;

namespace DbServer.Application.Factories.Database;

public class SqlQueryServiceFactory : ISqlQueryServiceFactory
{
    private QueryServiceFactory? _queryServiceFactory = null;

    public ISqlQueryServiceFactory.IQueryServiceFactory Create()
    {
        if(_queryServiceFactory is null){
            _queryServiceFactory = new QueryServiceFactory();
            Console.WriteLine("New Query Service Factory Generated");
        }
        return _queryServiceFactory;
    }

    class QueryServiceFactory : ISqlQueryServiceFactory.IQueryServiceFactory
    {
        public ICanvasQueryService CanvasQueryService() => new CanvasQueryService();

        public IContributionQueryService ContributionQueryService() => new ContributionQueryService();

        public ICreationQueryService CreationQueryService() => new CreationQueryService();

        public IPixelQueryService PixelQueryService() => new PixelQueryService();

        public IUserQueryService UserQueryService() => new UserQuerySevice();
    }
}
