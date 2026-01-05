using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class RabbitMqDependencyInjectionExtensions
{
    private const string SectionName = "EventBus";

    public static IEventBusBuilder AddRabbitMqzEventBus(this IHostApplicationBuilder builder, string connectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddRabbitMQClient(connectionName);

        builder.Services.AddOpenTelemetry()
           .WithTracing(tracing =>
           {
               tracing.AddSource(RabbitMQTelemetry.ActivitySourceName);
           });

        builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection(SectionName));

        // Abstractions on top of the core client API
        builder.Services.AddSingleton<RabbitMQTelemetry>();
        builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

        builder.Services.AddSingleton<IHostedService>(sp => (RabbitMQEventBus)sp.GetRequiredService<IEventBus>());

        return new EventBusBuilder(builder.Services);
    }
    private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }
}

