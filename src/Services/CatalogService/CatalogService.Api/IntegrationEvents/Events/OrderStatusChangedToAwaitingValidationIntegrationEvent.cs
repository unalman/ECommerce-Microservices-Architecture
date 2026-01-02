using EventBus.Base.Events;

namespace CatalogService.Api.IntegrationEvents.Events
{
    public record OrderStatusChangedToAwaitingValidationIntegrationEvent(int OrderId, IEnumerable<OrderStockItem> OrderStockItems) : IntegrationEvent;
}
