using EventBus.Base.Abstraction;
using WebApp.Services.OrderStatus.Events;

namespace WebApp.Services.OrderStatus.EventHandling
{
    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler(
        OrderStatusNotificationService orderStatusNotificationService,
        ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger)
        : IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
    {
        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
        {
            logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

            await orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
        }
    }
}
