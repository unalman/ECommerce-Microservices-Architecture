using BasketService.Api.Core.Application.Repository;
using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Extensions;
using ServiceDefaults;
using System.Text.Json.Serialization;

namespace BasketService.Api.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationsServices(this IHostApplicationBuilder builder)
        {
            builder.AddDefaultAuthentication();

            builder.AddRedisClient("redis");

            builder.Services.AddSingleton<IBasketRepository, RedisBasketRepository>();

            builder.AddRabbitMqEventBus("eventbus")
                .AddSubscription<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>()
                .ConfigureJsonOptions(options => options.TypeInfoResolverChain.Add(IntegrationEventContext.Default));
        }
    }

    [JsonSerializable(typeof(OrderStartedIntegrationEvent))]
    partial class IntegrationEventContext : JsonSerializerContext
    {
    }
}
