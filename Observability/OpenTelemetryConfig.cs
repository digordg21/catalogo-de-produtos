// TO DO, REMOVER CONFIGURAÇÃO DO OPENTELEMETRY DO PROGRAM.CS E COLOCAR AQUI PARA DEIXAR O PROGRAM.CS MAIS LIMPO

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace Catalogo.ObservabilityLab.Observability;

public static class OpenTelemetryConfig
{
    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
        .ConfigureResource(resource =>
            resource.AddService(serviceName: "rod-api-lab"))
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter("rod-api-custom-metrics")
                .AddPrometheusExporter();
        })
        // Configurar tracing 
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddSource("Microsoft.Data.Sqlite") // para capturar queries do EF Core
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri("http://localhost:4317");
                });
        });

        return services;
    }
}