using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure;

namespace OrderService.Api.Infrastructure
{
    public class OrderingContextSeed : IDbSeeder<OrderingContext>
    {
        public async Task SeedAsync(OrderingContext context)
        {
            if (!context.CardTypes.Any())
            {
                context.CardTypes.AddRange(GetPredefinedCardTypes());
                await context.SaveChangesAsync();
            }
            await context.SaveChangesAsync();
        }

        private static IEnumerable<CardType> GetPredefinedCardTypes()
        {
            yield return new CardType { Id = 1, Name = "Amex" };
            yield return new CardType { Id = 2, Name = "Visa" };
            yield return new CardType { Id = 3, Name = "MasterCard" };
        }
    }
}
