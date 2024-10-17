using Azure.Identity;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using SRWorkerServer.Web;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddSignalR().AddAzureSignalR(options => {
    options.ConnectionString = builder.Configuration["SIGNALR-CONNECTION-STRING"];
});

//builder.Services.AddSingleton<IHubContext<PixelHub>>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapHub<PixelHub>("/pixelHub");

app.Run();
