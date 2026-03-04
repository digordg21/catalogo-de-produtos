using System.Diagnostics;

namespace Catalogo.ObservabilityLab.Observability;

public static class TelemetrySources
{
    public const string ServiceName = "ObservabilityLab.Api";

    public static readonly ActivitySource ActivitySource =
        new ActivitySource(ServiceName);
}