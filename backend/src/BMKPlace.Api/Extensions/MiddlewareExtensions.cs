using System;
using BMKPlace.Api.Hubs;
using BMKPlace.Api.Middleware;

namespace BMKPlace.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
    public static WebApplication ConfigurePipeline(this WebApplication app, IConfiguration configuration)
    {
        app.UseApiExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BMKPlace API V1"));
            // Optional: app.UseDeveloperExceptionPage();
        }else{
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        const string corsPolicyName = "AllowSpecificOrigin";
        app.UseCors(corsPolicyName);

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<PixelHub>("/hub/pixel");

        return app;
    }
}
