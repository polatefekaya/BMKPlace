using System;
using DbServer.Application.Factories.Database;
using DbServer.Application.Interfaces.Factories.Database;
using DbServer.Application.Interfaces.Services.Database.SqlQuery;
using DbServer.Application.Interfaces.Services.MessageQueue;
using DbServer.Application.Services;
using DbServer.Application.Services.Database.SqlQuery;
using Microsoft.Extensions.DependencyInjection;

namespace DbServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication (this IServiceCollection services){
        services.AddSingleton<ISqlQueryServiceFactory,SqlQueryServiceFactory>();

        services.AddScoped<IUserQueryService, UserQuerySevice>();
        services.AddScoped<ICanvasQueryService, CanvasQueryService>();
        services.AddScoped<IContributionQueryService, ContributionQueryService>();
        services.AddScoped<ICreationQueryService, CreationQueryService>();
        services.AddScoped<IPixelQueryService, PixelQueryService>();
        

        services.AddScoped<IMessageService, MessageService>();
        return services;
    }
}

