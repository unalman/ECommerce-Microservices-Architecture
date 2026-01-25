using EventBus.Base.Abstraction;
using WebHooks.API.Model;
using WebHooks.API.Services;

namespace WebHooks.API.IntegrationEvents
{
    public class OrderStatusChangedToPaidIntegrationEventHandler(
        IWebhooksRetriever retriever,
        IWebhooksSender sender,
        ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
    {
        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            var subscriptions = await retriever.GetSubscriptionOfType(WebhookType.OrderPaid);

            logger.LogInformation("Received OrderStatusChangedToShippedIntegrationEvent and got {SubscriptionsCount} subscriptions to process", subscriptions.Count());

            var whook = new WebhookData(WebhookType.OrderPaid, @event);

            await sender.SendAll(subscriptions, whook);
        }
    }
}
