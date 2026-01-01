using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
namespace OrderService.Infrastructure.EntityConfiguration
{
    public class BuyerEntityTypeConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.ToTable("buyers");

            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Id)
                .UseHiLo("buyerseq");

            builder.Property(x => x.IdentityGuid)
                .HasMaxLength(200);

            builder.HasIndex("IdentityGuid")
                .IsUnique(true);

            builder.HasMany(x => x.PaymentMethods)
                .WithOne();
        }
    }
}
