using MediatR;
using OrderService.Api.Application.IntegrationEvents;
using OrderService.Api.Application.IntegrationEvents.Events;
using OrderService.Api.Extensions;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.Events;

namespace OrderService.Api.Application.DomainEventHandlers
{
    public class OrderStatusChangedToAwaitingValidationDomainEventHandler : INotificationHandler<OrderStatusChangedToAwaitingValidationDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderStatusChangedToAwaitingValidationDomainEventHandler> _logger;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToAwaitingValidationDomainEventHandler(
            IOrderRepository orderRepository,
            ILogger<OrderStatusChangedToAwaitingValidationDomainEventHandler> logger,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _buyerRepository = buyerRepository;
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }
        public async Task Handle(OrderStatusChangedToAwaitingValidationDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            OrderingApiTrace.LogOrderStatusUpdated(_logger, domainEvent.OrderId, OrderStatus.AwaitingValidation);

            var order = await _orderRepository.GetAsync(domainEvent.OrderId);
            var buyer = await _buyerRepository.FindByIdAsync(order.BuyerId.Value);

            var orderStockList = domainEvent.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units));

            var integrationEvent = new OrderStatusChangedToAwaitingValidationIntegrationEvent(order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid, orderStockList);
            await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
        }
    }
}
