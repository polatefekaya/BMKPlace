using System;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RestServer.Application.Interfaces.Infrastructure;
using RestServer.Infrastructure.RabbitMQ;

namespace RestServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services){
        services.AddSingleton<IMessageClient, RabbitMqClient>();

        return services;
    }

}
