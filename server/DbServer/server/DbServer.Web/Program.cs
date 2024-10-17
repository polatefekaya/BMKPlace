using Serilog;
using DbServer.Infrastructure;
using DbServer.Application;
using DbServer.Domain.Data.Options;
using RabbitMQ.Client;
using DbServer.Application.Interfaces.Services.MessageQueue;
using DbServer.Web;


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

builder.Services.AddSingleton<IConnectionFactory>(sp => {
    return new ConnectionFactory{
        HostName = "localhost",
        UserName = "guest",
        Password = "guest",
        Port = 5672,
        VirtualHost = "/"
    };
});

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.Configure<MessageConsumerSettings>(builder.Configuration.GetSection("MessageConsumerSettings"));

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

builder.Services.AddHostedService<StartupTask>();

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
//This is a command line
app.Run();
