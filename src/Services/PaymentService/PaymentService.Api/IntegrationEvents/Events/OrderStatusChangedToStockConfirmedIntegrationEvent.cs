using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public record OrderStatusChangedToStockConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;
}
