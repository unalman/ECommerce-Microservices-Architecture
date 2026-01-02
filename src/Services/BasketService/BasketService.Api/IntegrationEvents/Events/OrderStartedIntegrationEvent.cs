using EventBus.Base.Events;

namespace BasketService.Api.IntegrationEvents.Events;

public record OrderStartedIntegrationEvent(string UserId) : IntegrationEvent;

