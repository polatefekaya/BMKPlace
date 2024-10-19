using Azure.Identity;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using SRWorkerServer.Web;
using SRWorkerServer.Application;
using SRWorkerServer.Infrastructure;
using SRWorkerServer.Domain.Data.Settings;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));


Uri keyValUri = new Uri("https://bmkplace-keys.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(keyValUri, new DefaultAzureCredential());

Log.Logger = new LoggerConfiguration().CreateLogger();

string insightsKey = builder.Configuration["APPINSIGHTSINSTRUMENTATIONKEY"]!;
builder.Configuration["Serilog:WriteTo:2:Args:instrumentationKey"] = insightsKey;

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});

builder.Services.AddApplicationInsightsTelemetry(options => {
    options.ConnectionString = insightsKey;
    options.EnableAdaptiveSampling = true;
    options.EnableRequestTrackingTelemetryModule = true;
    options.EnableAppServicesHeartbeatTelemetryModule = true;
    options.EnableHeartbeat = true;
    options.EnableDependencyTrackingTelemetryModule = true;
    options.EnableDiagnosticsTelemetryModule = true;
    options.EnablePerformanceCounterCollectionModule = true;
    options.EnableQuickPulseMetricStream = true;
    options.EnableAzureInstanceMetadataTelemetryModule = true;
});

string[] allowedOrigins = new[] { 
    "https://localhost:3000",
    "https://192.168.68.56:3000",
    "https://192.168.68.56",
    "https://localhost" };

builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy",builder => {
        builder.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});

builder.Services.AddSignalR().AddAzureSignalR(options => {
    options.ConnectionString = builder.Configuration["SIGNALR-CONNECTION-STRING"];
});

builder.Services.AddSingleton<IConnection>(sp => {
    IConnectionFactory factory = new ConnectionFactory(){
        HostName = builder.Configuration["RabbitMqSettings:HostName"],
        UserName = builder.Configuration["RabbitMqSettings:Username"],
        Password = builder.Configuration["RabbitMqSettings:Password"]
    };

    return factory.CreateConnection();
});

builder.Services.AddApplication()
                .AddInfrastructure();
//builder.Services.AddSingleton<IHubContext<PixelHub>>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseCors("CorsPolicy");

app.MapHub<PixelHub>("/pixelHub").RequireCors("CorsPolicy");

app.Run();
