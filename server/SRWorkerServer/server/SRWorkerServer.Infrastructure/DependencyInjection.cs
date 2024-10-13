using System;
using Microsoft.Extensions.DependencyInjection;
using SRWorkerServer.Application.Interfaces.Infrastructure;
using SRWorkerServer.Infrastructure.RabbitMq;
using SRWorkerServer.Infrastructure.SignalR;

namespace SRWorkerServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services){
        services.AddScoped<IPixelMessageBrokerService, PixelRabbitMqService>();
        services.AddScoped<IPixelRealtimeService, PixelSignalRService>();

        return services;
    }
}
