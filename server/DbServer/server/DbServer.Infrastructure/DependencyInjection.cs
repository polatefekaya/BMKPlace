using System;
using DbServer.Application.Interfaces.Repository;
using DbServer.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace DbServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure (this IServiceCollection services){
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
