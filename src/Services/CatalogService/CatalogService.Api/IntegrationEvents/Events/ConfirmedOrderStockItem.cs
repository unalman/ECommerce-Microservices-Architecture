namespace CatalogService.Api.IntegrationEvents.Events
{
    public record ConfirmedOrderStockItem(int ProductId, bool HasStock);
}
