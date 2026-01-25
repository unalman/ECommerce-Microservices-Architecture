using EventBus.Base.Abstraction;
using EventBus.Base.Extensions;
using FluentValidation;
using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using OrderService.Api.Application.Behaviors;
using OrderService.Api.Application.IntegrationEvents;
using OrderService.Api.Application.IntegrationEvents.EventHandling;
using OrderService.Api.Application.IntegrationEvents.Events;
using OrderService.Api.Application.Queries;
using OrderService.Api.Application.Validations;
using OrderService.Api.Infrastructure;
using OrderService.Api.Infrastructure.Services;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Idempotency;
using OrderService.Infrastructure.Repository;
using ServiceDefaults;

namespace OrderService.Api.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;

            // Add the authentication services to DI
            builder.AddDefaultAuthentication();

            services.AddDbContext<OrderingContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("OrderingDB"));
            });
            builder.EnrichNpgsqlDbContext<OrderingContext>();

            services.AddMigration<OrderingContext, OrderingContextSeed>();

            services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<OrderingContext>>();

            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            builder.AddRabbitMqEventBus("eventbus")
                .AddEventBusSubscription();

            services.AddHttpContextAccessor();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });

            // Register the command validators for the validator behavior (validators based on FluentValidation library)
            services.AddValidatorsFromAssemblyContaining<CancelOrderCommandValidator>();

            services.AddScoped<IOrderQueries, OrderQueries>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IBuyerRepository, BuyerRepository>();
            services.AddScoped<IRequestManager, RequestManager>();
        }
        private static void AddEventBusSubscription(this IEventBusBuilder eventBus)
        {
            eventBus.AddSubscription<GracePeriodConfirmedIntegrationEvent, GracePeriodConfirmedIntegrationEventHandler>();
            eventBus.AddSubscription<OrderStockConfirmedIntegrationEvent, OrderStockConfirmedIntegrationEventHandler>();
            eventBus.AddSubscription<OrderStockRejectedIntegrationEvent, OrderStockRejectedIntegrationEventHandler>();
            eventBus.AddSubscription<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();
            eventBus.AddSubscription<OrderPaymentSucceededIntegrationEvent, OrderPaymentSucceededIntegrationEventHandler>();
        }
    }
}
