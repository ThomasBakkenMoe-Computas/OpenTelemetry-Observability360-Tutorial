using System.Runtime.Intrinsics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string serviceName = "relay-service";
const string serviceVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);

string otelCollectorUrl = builder.Configuration["AppSettings:oTelCollectorUrl"];

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName, serviceVersion))
        .AddConsoleExporter();
});

Action<ResourceBuilder> appResourceBuilder =
resource => resource
    .AddService(serviceName, serviceVersion);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(appResourceBuilder)
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(options => 
        {
            options.Endpoint = new Uri($"{otelCollectorUrl}/v1/traces");
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(options => {
            options.Endpoint = new Uri(otelCollectorUrl);
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })
        .AddConsoleExporter());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
