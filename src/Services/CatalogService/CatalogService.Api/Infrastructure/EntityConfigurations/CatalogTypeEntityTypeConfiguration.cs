using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfigurations
{
    public class CatalogTypeEntityTypeConfiguration : IEntityTypeConfiguration<CatalogType>
    {
        public void Configure(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType", CatalogContext.DEFAULT_SCHEMA);

            builder.Property(x => x.Id)
                .UseHiLo("catalog_hilo", CatalogContext.DEFAULT_SCHEMA)
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired();
        }
    }
}
