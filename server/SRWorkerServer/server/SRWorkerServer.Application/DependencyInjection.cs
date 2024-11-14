using System;
using Microsoft.Extensions.DependencyInjection;
using SRWorkerServer.Application.Interfaces;
using SRWorkerServer.Application.Services;

namespace SRWorkerServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services){
        services.AddScoped<IPixelService, PixelService>();
        services.AddScoped<ICorePixelService, CorePixelService>();

        return services;
    }
}
