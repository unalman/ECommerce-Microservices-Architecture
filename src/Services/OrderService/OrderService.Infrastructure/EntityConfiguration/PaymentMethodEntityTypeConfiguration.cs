using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;

namespace OrderService.Infrastructure.EntityConfiguration
{
    class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("paymentmethods");

            builder.Ignore(x => x.DomainEvents);

            builder.Property(c => c.Id)
                .UseHiLo("paymentseq");

            builder.Property<int>("BuyerId");

            builder.Property("_cardHolderName")
                .HasColumnName("CardHolderName")
                .HasMaxLength(200);

            builder.Property("_alias")
                .HasColumnName("Alias")
                .HasMaxLength(200);

            builder.Property("_cardNumber")
                .HasColumnName("Alias")
                .HasMaxLength(25)
                .IsRequired();

            builder.Property("_cardTypeId")
                .HasColumnName("CardTypeId");

            builder.HasOne(x => x.CardType)
                .WithMany()
                .HasForeignKey("_cardTypeId");
        }
    }
}
