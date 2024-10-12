using System;
using Microsoft.Extensions.DependencyInjection;
using RestServer.Application.Interfaces;
using RestServer.Application.Services;
using RestServer.Application.Services.Controllers;

namespace RestServer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services){
        services.AddSingleton<IMessagePublishDictionaryService, MessagePublishDictionaryService>();
        services.AddSingleton<IChannelManagerService, ChannelManagerService>();
        services.AddSingleton<IRequestService, RequestService>();
        services.AddSingleton<ControllerHelper>();
        return services;
    }
}
