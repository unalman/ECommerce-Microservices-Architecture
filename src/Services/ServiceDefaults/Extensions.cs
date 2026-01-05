using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ServiceDefaults
{
    public static partial class Extensions
    {
        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        {
            builder.AddBasicServicesDefaults();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            });
            return builder;
        }

        public static IHostApplicationBuilder AddBasicServicesDefaults(this IHostApplicationBuilder builder)
        {
            builder.AddDefaultHealthCheck();
            builder.ConfigureOpenTelemetry();

            return builder;
        }
        public static IHostApplicationBuilder AddDefaultHealthCheck(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]); // Add a default liveness check to ensure app is responsive
            return builder;
        }
        public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
        {
            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("Experimental.Microsoft.Extensions.AI");
                })
                .WithTracing(tracing =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        tracing.SetSampler(new AlwaysOnSampler());
                    }

                    tracing.AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("Experimental.Microsoft.Extension.AI");
                });

            builder.AddOpenTelemetryExporters();
            return builder;
        }
        private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
        {
            var useOtlExporter = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"));

            if (useOtlExporter)
            {
                builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
                builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
                builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());
            }
            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapHealthChecks("/health");
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });
            }
            return app;
        }
    }
}
