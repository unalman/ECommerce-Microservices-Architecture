using EventBus.Base.Extensions;
using OrderProcessor.Events;
using OrderProcessor.Services;
using System.Text.Json.Serialization;

namespace OrderProcessor.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            builder.AddRabbitMqEventBus("eventbus")
                .ConfigureJsonOptions(options => options.TypeInfoResolverChain.Add(IntegrationEventContext.Default));

            builder.AddNpgsqlDataSource("OrderingDB");

            builder.Services.AddOptions<BackgroundTaskOptions>()
                .BindConfiguration(nameof(BackgroundTaskOptions));

            builder.Services.AddHostedService<GracePeriodManagerService>();
        }
    }
    [JsonSerializable(typeof(GracePeriodConfirmedIntegrationEvent))]
    partial class IntegrationEventContext : JsonSerializerContext
    {

    }
}
