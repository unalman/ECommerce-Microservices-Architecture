using EventBus.Base.Events;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Api.Application.IntegrationEvents.Events
{
    public record OrderStatusChangedToSubmittedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string BuyerName { get; set; }
        public string BuyerIdentityGuid { get; set; }

        public OrderStatusChangedToSubmittedIntegrationEvent(int orderId, OrderStatus orderStatus, string buyerName, string buyerIdentityGuid)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
            BuyerIdentityGuid = buyerIdentityGuid;
        }
    }
}
