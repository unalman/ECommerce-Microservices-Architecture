namespace BasketService.Api.Core.Domain
{
    public class CustomerBasket
    {
        public string BuyerId { get; set; }

        public List<BasketItem> Items { get; set; } = [];

        public CustomerBasket() { }

        public CustomerBasket(string customerId)
        {
            BuyerId = customerId;
        }
    }
}
