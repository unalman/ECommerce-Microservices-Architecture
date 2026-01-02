using EventBus.Base.Events;

namespace CatalogService.Api.IntegrationEvents.Events
{
    public record OrderStatusChangedToPaidIntegrationEvent(int OrderId, IEnumerable<OrderStockItem> OrderStockItems) : IntegrationEvent;
}
