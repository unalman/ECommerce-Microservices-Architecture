using EventBus.Base.Events;

namespace CatalogService.Api.IntegrationEvents.Events
{
    public record ProductPriceChangedIntegrationEvent(int ProductId, decimal NewPrice, decimal OldPrice) : IntegrationEvent;
}
