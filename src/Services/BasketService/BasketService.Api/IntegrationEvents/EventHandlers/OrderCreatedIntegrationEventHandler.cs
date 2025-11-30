using BasketService.Api.Core.Application.Repository;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;

namespace BasketService.Api.IntegrationEvents.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler(IBasketRepository repository,
        ILogger<OrderCreatedIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

            await repository.DeleteBasketAsync(@event.UserId);
        }
    }
}
