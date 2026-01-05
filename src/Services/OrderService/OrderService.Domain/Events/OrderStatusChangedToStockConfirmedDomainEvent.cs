using MediatR;

namespace OrderService.Domain.Events
{
    public class OrderStatusChangedToStockConfirmedDomainEvent : INotification
    {
        public int OrderId { get; }

        public OrderStatusChangedToStockConfirmedDomainEvent(int orderId)
         => OrderId = orderId;
    }
}
