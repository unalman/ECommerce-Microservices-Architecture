using IdentityService.Api.Data;
using IdentityService.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Api.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationService(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSQLConnection")));

            if (builder.Environment.IsDevelopment())
                builder.Services.AddMigration<ApplicationDbContext, UsersSeed>();
        }
    }
}
