using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Infrastructure.EntityConfiguration
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.Ignore(c => c.DomainEvents);

            builder.Property(x => x.Id)
                .UseHiLo("orderseq");

            builder.OwnsOne(x => x.Address);

            builder.Property(x => x.OrderStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(x => x.PaymentId)
                .HasColumnName("PaymentMethodId");

            builder.HasOne<PaymentMethod>()
                .WithMany()
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Buyer)
                .WithMany()
                .HasForeignKey(x => x.BuyerId);
        }
    }
}
