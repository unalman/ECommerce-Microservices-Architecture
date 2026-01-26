using EventBus.Base.Abstraction;
using WebApp.Services.OrderStatus.Events;

namespace WebApp.Services.OrderStatus.EventHandling
{
    public class OrderStatusChangedToShippedIntegrationEventHandler(
     OrderStatusNotificationService orderStatusNotificationService,
     ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger)
     : IIntegrationEventHandler<OrderStatusChangedToShippedIntegrationEvent>
    {
        public async Task Handle(OrderStatusChangedToShippedIntegrationEvent @event)
        {
            logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
            await orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
        }

    }
}
