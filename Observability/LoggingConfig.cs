// TO DO, REMOVER CONFIGURAÇÃO DO LOGGING DO PROGRAM.CS E COLOCAR AQUI PARA DEIXAR O PROGRAM.CS MAIS LIMPO
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.Extensions.Hosting;
using Serilog.Enrichers.Span;

namespace Catalogo.ObservabilityLab.Observability;

public static class LoggingConfig
{
    public static IHostBuilder UseCustomSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithSpan() // <- pega TraceId automaticamente
                .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
                .WriteTo.File(
                    new Serilog.Formatting.Json.JsonFormatter(),
                    "logs/log-.json",
                    rollingInterval: RollingInterval.Day);
        });
    }
}