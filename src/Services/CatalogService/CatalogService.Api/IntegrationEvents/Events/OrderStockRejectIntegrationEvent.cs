using EventBus.Base.Events;

namespace CatalogService.Api.IntegrationEvents.Events
{
    public record OrderStockRejectIntegrationEvent(int OrderId, List<ConfirmedOrderStockItem> OrderStockItems) : IntegrationEvent;
}
