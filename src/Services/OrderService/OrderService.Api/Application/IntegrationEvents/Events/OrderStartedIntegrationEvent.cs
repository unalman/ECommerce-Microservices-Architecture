using EventBus.Base.Events;

namespace OrderService.Api.Application.IntegrationEvents.Events
{
    public record OrderStartedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; set; }
        public OrderStartedIntegrationEvent(string userId)
        {
            UserId = userId;
        }
    }
}
