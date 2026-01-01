using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;
using OrderService.Domain.Exceptions;
using OrderService.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.AggregateModels.OrderAggregate
{
    public class Order : Entity, IAggregateRoot
    {
        public DateTime OrderDate { get; private set; }
        [Required]
        public Address Address { get; private set; }
        public int? BuyerId { get; private set; }

        public Buyer Buyer { get; }

        public OrderStatus OrderStatus { get; private set; }

        public string Description { get; private set; }

        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        public int? PaymentId { get; private set; }

        private bool _isDraft;
        public static Order NewDraft()
        {
            var order = new Order
            {
                _isDraft = true
            };
            return order;
        }
        protected Order()
        {
            _orderItems = new List<OrderItem>();
            _isDraft = false;
        }

        public Order(string userId, string userName, Address address, int cardTypeId, string cardNumber, string cardSecurityNumber,
           string cardHolderName, DateTime cardExpiration, int? buyerId = null, int? paymentMethodId = null) : this()
        {
            BuyerId = buyerId;
            PaymentId = paymentMethodId;
            OrderStatus = OrderStatus.Submitted;
            OrderDate = DateTime.UtcNow;
            Address = address;

            // Add the OrderStarterDomainEvent to the domain events collection 
            // to be raised/dispatched when committing changes into the Database [ After DbContext.SaveChanges() ]
            AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber,
                                        cardSecurityNumber, cardHolderName, cardExpiration);
        }

        private void AddOrderStartedDomainEvent(string userId, string userName, int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,
                                                                        cardNumber, cardSecurityNumber,
                                                                        cardHolderName, cardExpiration);

            this.AddDomainEvent(orderStartedDomainEvent);
        }

        public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
        {
            var existingOrderForProduct = _orderItems.SingleOrDefault(x => x.ProductId == productId);
            if (existingOrderForProduct != null)
            {
                if (discount > existingOrderForProduct.Discount)
                    existingOrderForProduct.SetNewDiscount(discount);

                existingOrderForProduct.AddUnits(units);
            }
            else
            {
                var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
                _orderItems.Add(orderItem);
            }
        }

        public void SetPaymentMethodVerified(int buyerId, int paymentId)
        {
            BuyerId = buyerId;
            PaymentId = paymentId;
        }

        public void SetCancelledStatus()
        {
            if (OrderStatus == OrderStatus.Paid || OrderStatus == OrderStatus.Shipped)
            {
                StatusChangeException(OrderStatus.Cancelled);
            }

            OrderStatus = OrderStatus.Cancelled;
            Description = "The order was cancelled";
            AddDomainEvent(new OrderCancelledDomainEvent(this));
        }

        public void SetShippedStatus()
        {
            if (OrderStatus != OrderStatus.Paid)
            {
                StatusChangeException(OrderStatus.Shipped);
            }

            OrderStatus = OrderStatus.Shipped;
            Description = "The order was shipped";
            AddDomainEvent(new OrderShippedDomainEvent(this));
        }

        public decimal GetTotal() => _orderItems.Sum(o => o.Units * o.UnitPrice);

        private void StatusChangeException(OrderStatus orderStatusToChange)
        {
            throw new OrderingDomainException($"Is not possible to change the order status from {OrderStatus} to {orderStatusToChange}.");
        }
    }
}
