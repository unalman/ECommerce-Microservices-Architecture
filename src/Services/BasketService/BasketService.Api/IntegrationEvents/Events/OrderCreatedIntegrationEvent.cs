using EventBus.Base.Events;

namespace BasketService.Api.IntegrationEvents.Events;

public record OrderCreatedIntegrationEvent(string UserId) : IntegrationEvent;

