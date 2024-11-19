using System;
using RabbitMQ.Client;
using SRWorkerServer.Domain.Data.Settings;

namespace SRWorkerServer.Web;

public static class MessageBroker
{
    public static IServiceCollection AddSingletonConnection(this IServiceCollection services, RabbitMqSettings? settings)
    {
        if (settings is null) return services;
        
        services.AddSingleton<IConnection>(sp =>
        {
            IConnectionFactory factory = new ConnectionFactory()
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password
            };

            return factory.CreateConnection();
        });
        return services;
    }
}
