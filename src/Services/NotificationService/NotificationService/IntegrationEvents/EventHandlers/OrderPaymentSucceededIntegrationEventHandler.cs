using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using PaymentService.Api.IntegrationEvents.Events;

namespace NotificationService.IntegrationEvents.EventHandlers
{
    public class OrderPaymentSucceededIntegrationEventHandler(ILogger<OrderPaymentSucceededIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
    {
        public Task Handle(OrderPaymentSucceededIntegrationEvent @event)
        {
            logger.LogInformation($"Order Payment Succeeded with OrderId: {@event.OrderId}");

            return Task.CompletedTask;
        }
    }
}
