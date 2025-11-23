using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationService(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<CatalogContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSQLConnection")));

            if (builder.Environment.IsDevelopment())
                builder.Services.AddMigration<CatalogContext, CatalogContextSeed>();
        }
    }
}
