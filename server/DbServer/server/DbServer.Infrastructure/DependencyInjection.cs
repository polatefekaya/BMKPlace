using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Application.Interfaces.Services.Database;
using DbServer.Application.Interfaces.Services.MessageQueue;
using DbServer.Application.Interfaces.UnitOfWork;
using DbServer.Infrastructure.RabbitMQ;
using DbServer.Infrastructure.Repository;
using DbServer.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DbServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure (this IServiceCollection services){
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepositoryHelper, RepositoryHelper>();
        services.AddScoped<IDbServiceHelper, DbServiceHelper>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPixelRepository,PixelRepository>();
        services.AddScoped<ICreatorRepository, CreatorRepository>();
        services.AddScoped<IContributionRepository, ContributionRepository>();
        services.AddScoped<ICanvasRepository, CanvasRepository>();

        services.AddScoped<ICanvasDbService, CanvasDbService>();
        services.AddScoped<IContributionDbService, ContributionDbService>();
        services.AddScoped<ICreatorDbService, CreatorDbService>();
        services.AddScoped<IPixelDbService, PixelDbService>();
        services.AddScoped<IUserDbService, UserDbService>();

        services.AddScoped<IMessageConsumerFactory, MessageConsumerFactory>();

        return services;
    }
}
