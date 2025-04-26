using System;
using System.Text;
using BMKPlace.Application.Contracts.Abstractions.Infrastructure;
using BMKPlace.Application.Contracts.Abstractions.Persistence;
using BMKPlace.Application.Contracts.Abstractions.Realtime;
using BMKPlace.Infrastructure.Caching;
using BMKPlace.Infrastructure.Identity;
using BMKPlace.Infrastructure.Options;
using BMKPlace.Infrastructure.Persistence;
using BMKPlace.Infrastructure.Persistence.Repositories;
using BMKPlace.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace BMKPlace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration){
        
        services.Configure<OtpOptions>(configuration.GetSection(OtpOptions.SectionName));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName)); // Add EmailOptions config
        // Add configuration for RedisOptions later

        // ... DbContext, Repositories, UnitOfWork, Identity registrations ...

        // --- Configure Authentication Services ---
        // ... TokenService, OtpService, UserLookupService, AllowedDomainChecker ...

        services.AddSingleton<IDateTimeService, SystemDateTimeService>();
        services.AddScoped<IEmailService, EmailService>();
        
        services.AddScoped<ICanvasRepository, CanvasRepository>();
        services.AddScoped<IPixelRepository, PixelRepository>();
        services.AddScoped<IColorPaletteRepository, ColorPaletteRepository>();
        services.AddScoped<ICanvasUserContextRepository, CanvasUserContextRepository>();
        services.AddScoped<ISchoolRepository, SchoolRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // ... Caching registration ...
        // --- Configure Caching ---
        // Remove IDistributedCache registration if present (AddStackExchangeRedisCache or AddDistributedMemoryCache)
        // Register IConnectionMultiplexer for direct Redis access
        string? redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            //_logger.LogInformation("Configuring StackExchange Redis ConnectionMultiplexer."); // Assuming logger is available or add static logger
            try
            {
                // Use Singleton lifetime for IConnectionMultiplexer
                IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
                services.AddSingleton(redis);
                //_logger.LogInformation("Successfully connected to Redis and registered IConnectionMultiplexer.");
            }
            catch (RedisConnectionException ex)
            {
                 //_logger.LogError(ex, "Failed to connect to Redis using connection string from configuration. Redis-dependent services will fail.");
                 // Decide whether to throw, or let services fail later. Throwing might be better during startup.
                 throw new InvalidOperationException("Failed to establish Redis connection.", ex);
            }
        }
        else
        {
            //_logger.LogWarning("Redis connection string ('Redis') not found. RedisCanvasCache will not function correctly.");
            // Or throw if Redis is mandatory
            // throw new InvalidOperationException("Redis connection string 'Redis' not found in configuration.");

            // If Redis isn't mandatory, maybe register a NoOpCache implementation?
            // services.AddScoped<ICanvasCache, NoOpCanvasCache>(); // Example fallback
        }

                // --- Configure Identity ---
        services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
        {
            // ... identity options ...
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddRoles<IdentityRole<int>>(); // *** Ensure AddRoles<TRole>() is called ***

        services.AddMemoryCache();
        // Register the new ICanvasCache implementation (which now depends on IConnectionMultiplexer)
        services.AddScoped<ICanvasCache, RedisCanvasCache>();
        // --- Configure Real-time Services ---
        //services.AddScoped<IPixelNotifier, PixelNotifier>();

        // TODO: Register ICanvasCache implementation when created

        return services;
    }
    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

        if (string.IsNullOrWhiteSpace(jwtOptions.Secret) || string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience))
        {
            throw new InvalidOperationException("JWT configuration (Secret, Issuer, Audience) is missing or invalid.");
        }

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                ClockSkew = TimeSpan.Zero
            };

            // Configure SignalR JWT auth from query string
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub/pixel")) // Use hub path
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}
