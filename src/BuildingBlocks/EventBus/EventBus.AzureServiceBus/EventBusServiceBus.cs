using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus
    {
        private ServiceBusClient serviceBusClient;
        private ServiceBusSender serviceBusSender;
        private ServiceBusAdministrationClient serviceBusAdministrationClient;
        private ILogger logger;
        public EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            logger = ServiceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
            serviceBusAdministrationClient = new ServiceBusAdministrationClient(config.EventBusConectionString);
            serviceBusClient = CraeteSubscriptionClient();
            serviceBusSender = CreateServiceBusSenderAsync(serviceBusClient).GetAwaiter().GetResult();
        }
        private async Task<ServiceBusSender> CreateServiceBusSenderAsync(ServiceBusClient serviceBusClient)
        {
            if (serviceBusClient != null && !serviceBusClient.IsClosed)
            {
                if (serviceBusSender == null || serviceBusSender.IsClosed)
                    serviceBusSender = serviceBusClient.CreateSender(EventBusConfig.DefaultTopicName);
            }
            var isTopicExists = await serviceBusAdministrationClient.TopicExistsAsync(EventBusConfig.DefaultTopicName);
            if (!isTopicExists.Value)
                await serviceBusAdministrationClient.CreateTopicAsync(EventBusConfig.DefaultTopicName);

            return serviceBusSender;
        }

        private ServiceBusClient CraeteSubscriptionClient()
        {
            if (serviceBusClient == null || serviceBusClient.IsClosed)
            {
                serviceBusClient = new ServiceBusClient(EventBusConfig.EventBusConectionString,
                    options: new ServiceBusClientOptions() { RetryOptions = new ServiceBusRetryOptions() { MaxRetries = EventBusConfig.ConnectionRetryCount }, });
            }
            return serviceBusClient;
        }

        public override async Task Publish(IntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            var eventName = @event.GetType().Name; // example: OrderCreatedIntegrationEvent
            //eventName = ProcessEventName(eventName);// example: OrderCreated

            var evetStr = JsonConvert.SerializeObject(@event);
            var bodyArr = Encoding.UTF8.GetBytes(evetStr);
            var message = new ServiceBusMessage()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = new BinaryData(bodyArr),
                Subject = eventName
            };
            await serviceBusSender.SendMessageAsync(message, cancellationToken);
        }

        public override async Task Subscribe<T, TH>(CancellationToken cancellationToken = default)
        {
            var eventName = typeof(T).Name;
            //eventName = ProcessEventName(eventName);
            if (!SubsManager.HasSubscriptionForEvent(eventName))
            {
                await CreateServiceBusAdministrationClient(eventName, cancellationToken);
                await RegisterSubscriptionClientMessageHandler(eventName, cancellationToken);
            }

            logger.LogInformation($"Subscribing to event {eventName} with {typeof(TH).Name}");

            SubsManager.AddSubscription<T, TH>();
        }
        private async Task RegisterSubscriptionClientMessageHandler(string eventName, CancellationToken cancellationToken = default)
        {
            var options = new ServiceBusProcessorOptions()
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 10

            }; //EventBusConfig.DefaultTopicName, GetSubName(eventName)
            ServiceBusProcessor processor = serviceBusClient.CreateProcessor(EventBusConfig.DefaultTopicName, GetSubName(eventName));
            processor.ProcessMessageAsync += async args =>
            {
                var eventName = args.Message.Subject;
                var messageData = args.Message.Body.ToString();

                if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    await args.CompleteMessageAsync(args.Message);
                else
                    await args.AbandonMessageAsync(args.Message);
            };

            processor.ProcessErrorAsync += args =>
            {
                logger.LogError(args.Exception, "Service Bus processing error");
                return Task.CompletedTask;
            };
            await processor.StartProcessingAsync(cancellationToken);
        }
        private async Task CreateServiceBusAdministrationClient(string eventName, CancellationToken cancellationToken = default)
        {
            var exists = await serviceBusAdministrationClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), cancellationToken);
            if (!exists.Value)
            {
                await serviceBusAdministrationClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), cancellationToken);
                await RemoveDefaultRule(eventName, cancellationToken);
            }
            await CreateRuleIfNotExists(eventName, cancellationToken);
        }
        private async Task CreateRuleIfNotExists(string eventName, CancellationToken cancellationToken = default)
        {
            var exists = await serviceBusAdministrationClient.RuleExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), eventName, cancellationToken);
            if (!exists.Value)
            {
                var createRuleOptions = new CreateRuleOptions()
                {
                    Name = eventName,
                    Filter = new CorrelationRuleFilter { Subject = eventName }
                };
                await serviceBusAdministrationClient.CreateRuleAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), createRuleOptions, cancellationToken);
            }
        }
        private async Task RemoveDefaultRule(string eventName, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = await serviceBusAdministrationClient.RuleExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), RuleProperties.DefaultRuleName, cancellationToken);
                if (exists.Value)
                    await serviceBusAdministrationClient.DeleteRuleAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), RuleProperties.DefaultRuleName, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning("The messaging entity {DefaultRuleName} Could not be found.", RuleProperties.DefaultRuleName);
            }
        }

        public override async Task UnSubscribe<T, TH>(CancellationToken cancellationToken = default)
        {
            var eventName = typeof(T).Name;
            try
            {
                var exists = await serviceBusAdministrationClient.RuleExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), eventName, cancellationToken);
                if (exists.Value)
                    await serviceBusAdministrationClient.DeleteRuleAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), eventName, cancellationToken);
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
                logger.LogWarning("The messaging entity {eventName} Could not be found", eventName);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            serviceBusClient.DisposeAsync().GetAwaiter().GetResult();
            serviceBusClient = null;
            serviceBusSender.CloseAsync().GetAwaiter().GetResult();
            serviceBusSender = null;
            serviceBusAdministrationClient = null;
        }
    }
}
