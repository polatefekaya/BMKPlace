using Serilog;
using DbServer.Infrastructure;
using DbServer.Application;
using DbServer.Domain.Data.Options;
using RabbitMQ.Client;
using DbServer.Application.Interfaces.Services.MessageQueue;


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
        Port = AmqpTcpEndpoint.UseDefaultPort,
        VirtualHost = "/"
    };
});

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() => {
    using var scope = app.Services.CreateScope();
    var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
    messageService.Start();
});


//app.MapGet("/", () => "Hello World!");
//This is a command line
app.Run();
