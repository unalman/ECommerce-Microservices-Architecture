using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public record OrderPaymentFailedIntegrationEvent(int OrderId) : IntegrationEvent;
}
