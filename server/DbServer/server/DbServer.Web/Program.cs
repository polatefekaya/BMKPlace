using Serilog;
using DbServer.Infrastructure;
using DbServer.Application;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt", 
        rollingInterval: RollingInterval.Day, 
        rollOnFileSizeLimit: true, 
        fileSizeLimitBytes: 1_048_576)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
//This is a command line
app.Run();
