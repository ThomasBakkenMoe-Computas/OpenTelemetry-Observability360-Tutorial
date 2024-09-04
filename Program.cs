using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
const string serviceName = "relay-service";
const string serviceVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion))
        .AddConsoleExporter();
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
