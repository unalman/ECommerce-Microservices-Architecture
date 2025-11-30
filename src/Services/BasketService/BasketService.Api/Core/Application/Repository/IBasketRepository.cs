using BasketService.Api.Core.Domain;

namespace BasketService.Api.Core.Application.Repository
{
    public interface IBasketRepository
    {
        public Task<CustomerBasket> GetBasketAsync(string customerId);

        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);

        Task<bool> DeleteBasketAsync(string id);
    }
}
