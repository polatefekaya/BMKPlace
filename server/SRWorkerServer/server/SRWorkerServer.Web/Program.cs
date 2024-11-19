using Azure.Identity;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using SRWorkerServer.Web;
using SRWorkerServer.Application;
using SRWorkerServer.Infrastructure;
using SRWorkerServer.GridCache;
using SRWorkerServer.Domain.Data.Settings;
using RabbitMQ.Client;
using SRWorkerServer.Infrastructure.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

string insightsKey = builder.Configuration["APPINSIGHTSINSTRUMENTATIONKEY"]!;
builder.Configuration["Serilog:WriteTo:2:Args:instrumentationKey"] = insightsKey;

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));

builder.Configuration.ConfigureKeyVault();

Log.Logger = new LoggerConfiguration().CreateLogger();


builder.Host.ConfigureSerilog();

builder.Services.ConfigureTelemetry(insightsKey);

builder.Services.ConfigureCors();

builder.Services.ConfigureSignalR(builder.Configuration["SIGNALR-CONNECTION-STRING"]);

builder.Services.AddSingletonConnection(
    builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>()
);

builder.Services.AddApplication()
                .AddInfrastructure();
                
builder.Services.AddGridCache();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseCors("CorsPolicy");

app.MapHub<PixelHub>("/pixelHub").RequireCors("CorsPolicy");

app.Run();
