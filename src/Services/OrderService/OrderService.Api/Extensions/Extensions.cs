using Microsoft.EntityFrameworkCore;
using OrderService.Api.Application.Behaviors;
using OrderService.Api.Application.Queries;
using OrderService.Api.Infrastructure;
using OrderService.Api.Infrastructure.Services;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Repository;
using ServiceDefaults;

namespace OrderService.Api.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;

            builder.AddDefaultAuthentication();

            services.AddDbContext<OrderingContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionStrings"));
            });

            if (builder.Environment.IsDevelopment())
                services.AddMigration<OrderingContext, OrderingContextSeed>();

            services.AddHttpContextAccessor();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });

            services.AddScoped<IOrderQueries, OrderQueries>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IBuyerRepository, BuyerRepository>();
        }
    }
}
