using EventBus.Base.Events;

namespace EventBus.UnitTest.Events.Events
{
    public record OrderCreatedIntegrationEvent(int id): IntegrationEvent;
}
