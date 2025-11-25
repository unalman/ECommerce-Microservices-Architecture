using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using PaymentService.Api.IntegrationEvents.Events;

namespace NotificationService.IntegrationEvents.EventHandlers
{
    public class OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        public Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            logger.LogInformation($"Order Payment failed with OrderId: {@event.OrderId}");

            return Task.CompletedTask;
        }
    }
}
