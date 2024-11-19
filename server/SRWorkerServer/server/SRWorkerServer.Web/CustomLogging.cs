using System;
using Serilog;

namespace SRWorkerServer.Web;

public static class CustomLogging
{
    public static IHostBuilder ConfigureSerilog(this IHostBuilder host)
    {
        host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
        {
            loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
        });
        return host;
    }
}
