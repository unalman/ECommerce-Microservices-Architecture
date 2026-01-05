using MediatR;
using OrderService.Domain.AggregateModels.OrderAggregate;

/// <summary>
/// Event used when the grace period order is confirmed
/// </summary>
namespace OrderService.Domain.Events
{
    public class OrderStatusChangedToAwaitingValidationDomainEvent : INotification
    {
        public int OrderId { get; }
        public IEnumerable<OrderItem> OrderItems { get; }

        public OrderStatusChangedToAwaitingValidationDomainEvent(int orderId, IEnumerable<OrderItem> orderItems)
        {
            OrderId = orderId;
            OrderItems = orderItems;
        }
    }
}
