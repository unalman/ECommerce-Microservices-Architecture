using EventBus.Base.Events;

namespace WebHooks.API.IntegrationEvents
{
    public record OrderStatusChangedToPaidIntegrationEvent(int OrderId, IEnumerable<OrderStockItem> OrderStockItems) : IntegrationEvent;
}
