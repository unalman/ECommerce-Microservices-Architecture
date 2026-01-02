using CatalogService.Api.Infrastructure.Context;
using CatalogService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;

namespace CatalogService.Api.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToPaidIntegrationEventHandler(
        CatalogContext catalogContext,
        ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
    {
        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

            foreach (var orderStockItem in @event.OrderStockItems)
            {
                var catalogItem = catalogContext.CatalogItems.Find(orderStockItem.ProductId);
                catalogItem?.RemoveStock(orderStockItem.Units);
            }

            await catalogContext.SaveChangesAsync();
        }
    }
}
