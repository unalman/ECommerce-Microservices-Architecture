using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.UnitTest
{
    [TestClass]
    public class EventBusTests
    {
        private ServiceCollection services;

        public EventBusTests()
        {
            services = new ServiceCollection();
            services.AddLogging(configure => configure.AddConsole());
        }
        [TestMethod]
        public async Task Subscribe_Event_On_RabbitMQ_Test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });
            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            await eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            await eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }
        private EventBusConfig GetRabbitMQConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 1,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "EcommerceExchangeTopicName",
                EventBusType = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent",
                Connection = new ConnectionFactory()
                {
                    HostName = "localhost",
                    Port = 5672,
                    UserName = "unal",
                    Password = "unal"
                }
            };
        }
        [TestMethod]
        public async Task Send_Message_To_RabbitM_Test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });
            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            await eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }
    }
}
