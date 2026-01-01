using Microsoft.EntityFrameworkCore;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.SeedWork;

namespace OrderService.Infrastructure.Repository
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly OrderingContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public BuyerRepository(OrderingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Buyer Add(Buyer buyer)
        {
            if (buyer.IsTransient())
            {
                return _context.Buyers
                    .Add(buyer)
                    .Entity;
            }
            return buyer;
        }
        public Buyer Update(Buyer buyer)
        {
            return _context.Buyers
                .Update(buyer)
                .Entity;
        }

        public async Task<Buyer> FindAsync(string BuyerIdentityGuid)
        {
            var buyer = await _context.Buyers
                .Include(x => x.PaymentMethods)
                .Where(x => x.IdentityGuid == BuyerIdentityGuid)
                .SingleOrDefaultAsync();

            return buyer;
        }

        public async Task<Buyer> FindByIdAsync(int id)
        {
            var buyer = await _context.Buyers
                .Include(x => x.PaymentMethods)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            return buyer;
        }
    }
}
