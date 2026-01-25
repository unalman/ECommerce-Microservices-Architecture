using EventBus.Base.Events;

namespace WebHooks.API.IntegrationEvents
{
    public record OrderStatusChangedToShippedIntegrationEvent(int OrderId, string OrderStatus, string BuyerName) : IntegrationEvent;
}
