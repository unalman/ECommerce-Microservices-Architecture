using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.EntityConfigurations;
using IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CatalogService.Api.Infrastructure.Context
{
    public class CatalogContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "catalog";

        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.HasDefaultSchema("catalog");

            builder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
            builder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
            builder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());

            builder.UseIntegraitonEventLogs();
        }
    }
}
