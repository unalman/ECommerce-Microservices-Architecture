using EventBus.Base.Events;

namespace OrderService.Api.Application.IntegrationEvents.Events
{
    public record OrderStockRejectedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public List<ConfirmedOrderStockItem> OrderStockItems { get; }
    }

    public record ConfirmedOrderStockItem
    {
        public int ProductId { get; }
        public bool HasStock { get; }

        public ConfirmedOrderStockItem(int productId, bool hasStock)
        {
            ProductId = productId;
            HasStock = hasStock;
        }
    }
}
