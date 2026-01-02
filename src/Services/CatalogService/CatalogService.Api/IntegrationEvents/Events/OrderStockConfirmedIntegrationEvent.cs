using EventBus.Base.Events;

namespace CatalogService.Api.IntegrationEvents.Events
{
    public record OrderStockConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;
}
