using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public record OrderPaymentSucceededIntegrationEvent(int OrderId) : IntegrationEvent;
}
