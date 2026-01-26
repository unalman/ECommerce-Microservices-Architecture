using EventBus.Base.Abstraction;
using WebApp.Services.OrderStatus.Events;

namespace WebApp.Services.OrderStatus.EventHandling
{
    public class OrderStatusChangedToPaidIntegrationEventHandler(
    OrderStatusNotificationService orderStatusNotificationService,
    ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger)
    : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
    {
        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
            await orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
        }
    }
}
