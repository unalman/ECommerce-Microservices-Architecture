using MediatR;
using OrderService.Api.Application.IntegrationEvents;
using OrderService.Api.Application.IntegrationEvents.Events;
using OrderService.Api.Extensions;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;

namespace OrderService.Api.Application.DomainEventHandlers
{
    public class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler
        : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly ILogger _logger;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(ILogger logger,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _logger = logger;
            _buyerRepository = buyerRepository;
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStartedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var cardTypeId = domainEvent.CardTypeId != 0 ? domainEvent.CardTypeId : 1;
            var buyer = await _buyerRepository.FindAsync(domainEvent.UserId);
            var buyerExisted = buyer is not null;

            if (!buyerExisted)
            {
                buyer = new Buyer(domainEvent.UserId, domainEvent.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(cardTypeId,
                $"Payment Method on {DateTime.UtcNow}",
                domainEvent.CardNumber,
                domainEvent.CardSecurityNumber,
                domainEvent.CardHolderName,
                domainEvent.CardExpiration,
                domainEvent.Order.Id);

            if (!buyerExisted)
            {
                _buyerRepository.Add(buyer);
            }

            await _buyerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            var integrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(domainEvent.Order.Id, domainEvent.Order.OrderStatus, buyer.Name, buyer.IdentityGuid);
            await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
            OrderingApiTrace.LogOrderBuyerAndPaymentValidatedOrUpdated(_logger, buyer.Id, domainEvent.Order.Id);
        }
    }
}
