using EventBus.Base.Extensions;
using PaymentService.Api;
using PaymentService.Api.IntegrationEvents.EventHandlers;
using PaymentService.Api.IntegrationEvents.Events;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRabbitMqzEventBus("EventBus")
    .AddSubscription<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();

builder.Services.AddOptions<PaymentOptions>()
    .BindConfiguration(nameof(PaymentOptions));

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
