using Microsoft.EntityFrameworkCore;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.SeedWork;

namespace OrderService.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderingContext _context;
        public IUnitOfWork UnitOfWork => _context;
        public OrderRepository(OrderingContext context)
        {
            context = _context ?? throw new ArgumentNullException(nameof(context));
        }

        public Order Add(Order order)
        {
            return _context.Orders.Add(order).Entity;
        }

        public async Task<Order> GetAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                await _context.Entry(order)
                    .Collection(x => x.OrderItems).LoadAsync();
            }
            return order;
        }

        public void Update(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
        }
    }
}
