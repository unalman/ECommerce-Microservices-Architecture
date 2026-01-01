using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure;

namespace OrderService.Api.Application.Queries
{
    public class OrderQueries(OrderingContext context) : IOrderQueries
    {
        public async Task<Order> GetOrderAsync(int id)
        {
            var order = await context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order is null) throw new KeyNotFoundException();

            return new Order
            {
                OrderNumber = order.Id,
                Date = order.OrderDate,
                Description = order.Description,
                City = order.Address.City,
                Country = order.Address.Country,
                State = order.Address.State,
                Street = order.Address.Street,
                Zipcode = order.Address.ZipCode,
                Status = order.OrderStatus.ToString(),
                Total = order.GetTotal(),
                OrderItems = order.OrderItems.Select(oi => new Orderitem
                {
                    ProductName = oi.ProductName,
                    Units = oi.Units,
                    UnitPrice = (double)oi.UnitPrice,
                    PictureUrl = oi.PictureUrl
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(string userId)
        {
            return await context.Orders
                .Where(x => x.Buyer.IdentityGuid == userId)
                .Select(o => new OrderSummary
                {
                    OrderNumber = o.Id,
                    Date = o.OrderDate,
                    Status = o.OrderStatus.ToString(),
                    Total = (double)o.OrderItems.Sum(oi => oi.UnitPrice * oi.Units)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CardType>> GetCardTypesAsync()
        {
            return await context.CardTypes.Select(x => new CardType { Id = x.Id, Name = x.Name }).ToListAsync();
        }
    }
}
