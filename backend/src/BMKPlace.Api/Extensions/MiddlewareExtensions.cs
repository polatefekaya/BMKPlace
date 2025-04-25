using System;

namespace BMKPlace.Api.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app, IConfiguration configuration)
    {
        // app.UseMiddleware<ExceptionHandlerMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BMKPlace API V1"));
            // Optional: app.UseDeveloperExceptionPage();
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
