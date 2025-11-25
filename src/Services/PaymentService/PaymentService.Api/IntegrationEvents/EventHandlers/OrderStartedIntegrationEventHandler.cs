using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using PaymentService.Api.IntegrationEvents.Events;

namespace PaymentService.Api.IntegrationEvents.EventHandlers
{
    public class OrderStartedIntegrationEventHandler(IConfiguration configuration,
        IEventBus eventBus,
        ILogger<OrderStartedIntegrationEventHandler> logger)
        : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
            string keyword = "PaymentSuccess";
            bool paymentSuccessFlag = configuration.GetValue<bool>(keyword);

            IntegrationEvent paymentEvent = paymentSuccessFlag
                ? new OrderPaymentSucceededIntegrationEvent(@event.OrderId) : new OrderPaymentFailedIntegrationEvent(@event.OrderId);

            logger.LogInformation("Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", paymentEvent.Id, paymentEvent);

            eventBus.Publish(paymentEvent);

            return Task.CompletedTask;
        }
    }
}
