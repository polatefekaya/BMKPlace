using System;

namespace SRWorkerServer.Web;

public static class Realtime
{
    public static IServiceCollection ConfigureSignalR(this IServiceCollection services, string? connectionString)
    {
        if(connectionString is null) return services;

        services.AddSignalR().AddAzureSignalR(options =>
        {
            options.ConnectionString = connectionString;
        });
        
        return services;
    }
}
