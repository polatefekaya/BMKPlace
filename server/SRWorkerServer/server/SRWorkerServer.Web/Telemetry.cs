using System;

namespace SRWorkerServer.Web;

public static class Telemetry
{
    public static IServiceCollection ConfigureTelemetry(this IServiceCollection services, string connectionString)
    {
        services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = connectionString;
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
        return services;
    }
}
