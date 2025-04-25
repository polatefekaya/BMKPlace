using System;
using BMKPlace.Application.Contracts.Abstractions.Application.Pixels;
using BMKPlace.Application.Contracts.Abstractions.Validation;
using BMKPlace.Application.Features.Pixels.Commands.Shared;
using BMKPlace.Application.Features.Pixels.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace BMKPlace.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services){
        services.AddScoped<IPixelPlacementValidator, PixelPlacementValidator>();

        // Register the internal shared service (Scoped lifetime is appropriate)
        services.AddScoped<IPixelPlacementService, PixelPlacementService>();

        // Mediator registration usually happens in the API layer via AddMediator()

        return services;
    }
}
