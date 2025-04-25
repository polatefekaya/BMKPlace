using BMKPlace.Infrastructure.Options; // For JwtOptions
using Microsoft.AspNetCore.Authentication.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BMKPlace.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddSignalR();

        const string corsPolicyName = "AllowSpecificOrigin";
        services.AddCors(options =>
        {
            options.AddPolicy(name: corsPolicyName, policy =>
            {
                // IMPORTANT: Replace with your actual frontend origins
                string[] allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                                          ?? ["http://localhost:3000"];

                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });


        // Configure OpenAPI/Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "BMKPlace API", Version = "v1" });
            // Add JWT Auth UI support
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Add other presentation-layer services if needed (e.g., HealthChecks, ProblemDetails)

        return services;
    }
}
