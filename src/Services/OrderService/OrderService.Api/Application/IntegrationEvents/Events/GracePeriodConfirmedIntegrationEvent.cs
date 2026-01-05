using EventBus.Base.Events;

namespace OrderService.Api.Application.IntegrationEvents.Events
{
    public record GracePeriodConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public GracePeriodConfirmedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}
