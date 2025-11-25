using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.EventHandlers;
using PaymentService.Api.IntegrationEvents.Events;
using RabbitMQ.Client;



ServiceCollection services = new ServiceCollection();

ConfigureServices(services);

var sp = services.BuildServiceProvider();

IEventBus eventBus = sp.GetRequiredService<IEventBus>();
await eventBus.Subscribe<OrderPaymentSucceededIntegrationEvent, OrderPaymentSucceededIntegrationEventHandler>();
await eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();

Console.ReadLine();

static void ConfigureServices(ServiceCollection services)
{
    services.AddLogging(configure => configure.AddConsole());
    services.AddTransient<OrderPaymentSucceededIntegrationEventHandler>();
    services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
    services.AddSingleton<IEventBus>(sp =>
    {
        EventBusConfig config = new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "NotificationService",
            EventBusType = EventBusType.RabbitMQ,
            Connection = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "unal",
                Password = "unal"
            }
        };
        return EventBusFactory.Create(config, sp);
    });
}
