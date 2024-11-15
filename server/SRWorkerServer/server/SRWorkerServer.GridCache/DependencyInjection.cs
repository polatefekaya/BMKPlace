using System;
using Microsoft.Extensions.DependencyInjection;
using SRWorkerServer.GridCache.Interfaces;
using SRWorkerServer.GridCache.Services;

namespace SRWorkerServer.GridCache;

public static class DependencyInjection
{
    public static IServiceCollection AddGridCache(this IServiceCollection services)
    {
        services.AddSingleton<GridDictionary>();

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IGridCacheFactory, GridCacheFactory>();
        services.AddScoped<ICacheRepository, CacheRepository>();

        services.AddScoped<IBitwiseEngine, BitwiseEngine>();
        services.AddScoped<IBitwiseService, BitwiseService>();

        return services;
    }
}
