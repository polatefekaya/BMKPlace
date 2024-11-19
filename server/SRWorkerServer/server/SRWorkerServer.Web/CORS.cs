using System;

namespace SRWorkerServer.Web;

public static class CORS
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {

        string[] allowedOrigins = new[] {
            "https://localhost:3000",
            "https://192.168.68.56:3000",
            "https://192.168.68.56",
            "https://localhost"
        };

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
            });
        });
        return services;
    }
}
