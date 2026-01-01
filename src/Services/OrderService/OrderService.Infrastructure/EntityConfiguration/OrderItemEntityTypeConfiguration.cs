using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Infrastructure.EntityConfiguration
{
    class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("orderItems");

            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Id)
                .UseHiLo("orderitemseq");

            builder.Property<int>("OrderId");
        }
    }
}
