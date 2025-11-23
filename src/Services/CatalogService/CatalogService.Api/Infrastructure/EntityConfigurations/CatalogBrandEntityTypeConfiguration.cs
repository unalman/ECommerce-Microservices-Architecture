using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfigurations
{
    public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand", CatalogContext.DEFAULT_SCHEMA);
        
            builder.Property(x => x.Id)
                .UseHiLo("catalog_brand_hilo", CatalogContext.DEFAULT_SCHEMA)
                .IsRequired();

            builder.Property(x => x.Brand)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
