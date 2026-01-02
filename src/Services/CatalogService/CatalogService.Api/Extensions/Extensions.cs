using CatalogService.Api.Infrastructure.Context;
using CatalogService.Api.IntegrationEvents;
using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationService(this IHostApplicationBuilder builder)
        {
            // Avoid loading full database config and migrations if startup
            // is being invoked from build-time OpenAPI generation
            if (builder.Environment.IsBuild())
            {
                builder.Services.AddDbContext<CatalogContext>();
                return;
            }

            builder.Services.AddDbContext<CatalogContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSQLConnection")));

            if (builder.Environment.IsDevelopment())
                builder.Services.AddMigration<CatalogContext, CatalogContextSeed>();

            builder.Services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<CatalogContext>>();
            builder.Services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();
        }
    }
}
