using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using RestServer.Domain.Data.Settings;
using RestServer.Application;
using RestServer.Infrastructure;
using Serilog;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().CreateLogger();

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});

builder.Services.AddHttpLogging(options => {
    options.LoggingFields = 
    Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders 
    | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
});

builder.Services.AddControllers(options => {
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));
});

builder.Services.AddApiVersioning(config => {
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
})
    .AddVersionedApiExplorer(options => {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Version = "1.0", Title = "BMKPlace REST API V1"});
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Version = "2.0", Title = "BMKPlace REST API V2" });
});

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));

builder.Services.Configure<ClientSettings>(ClientSettings.User, builder.Configuration.GetSection($"ClientSettings:{ClientSettings.User}"));

builder.Services.Configure<ClientSettings>(ClientSettings.Canvas, builder.Configuration.GetSection($"ClientSettings:{ClientSettings.Canvas}"));

builder.Services
    .AddInfrastructure()
    .AddApplication();


builder.Services.AddSingleton<IConnection>(sp => {
    IConnectionFactory factory = new ConnectionFactory(){
        HostName = builder.Configuration["RabbitMqSettings:HostName"],
        UserName = builder.Configuration["RabbitMqSettings:Username"],
        Password = builder.Configuration["RabbitMqSettings:Password"]
    };

    return factory.CreateConnection();
});



var app = builder.Build();
app.UseHttpLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");
});
}

app.UseHsts();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
