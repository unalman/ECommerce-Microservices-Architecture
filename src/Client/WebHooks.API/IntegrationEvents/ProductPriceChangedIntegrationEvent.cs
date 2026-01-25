using EventBus.Base.Events;

namespace WebHooks.API.IntegrationEvents
{
    public record ProductPriceChangedIntegrationEvent(int ProductId, decimal NewPrice, decimal OldPrice) : IntegrationEvent;
}
