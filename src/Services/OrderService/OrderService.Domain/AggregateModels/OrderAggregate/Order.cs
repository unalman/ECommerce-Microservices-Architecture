using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;
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
    }
}
