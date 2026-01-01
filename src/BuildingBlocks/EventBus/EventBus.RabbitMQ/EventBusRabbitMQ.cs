using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        RabbitMQPersistentConnection persistentConnection;
        private readonly IConnectionFactory connectionfactory;
        private readonly IChannel consumerChannel;
        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            if (config.Connection != null)
            {
                var connJson = JsonConvert.SerializeObject(EventBusConfig, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                connectionfactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);

                if (config.Connection is ConnectionFactory connectionFactory)
                {
                    connectionfactory.UserName = connectionFactory.UserName;
                    connectionfactory.Password = connectionFactory.Password;
                }
            }
            else
                connectionfactory = new ConnectionFactory();

            persistentConnection = new RabbitMQPersistentConnection(connectionfactory, config.ConnectionRetryCount);
            consumerChannel = CreateConsumerChannel().GetAwaiter().GetResult();

            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object? sender, string eventName)
        {
            //eventName = ProcessEventName(eventName);

            if (!persistentConnection.IsConnected)
                persistentConnection.TryConnect();

            consumerChannel.QueueUnbindAsync(queue: eventName, exchange: EventBusConfig.DefaultTopicName, routingKey: eventName).GetAwaiter().GetResult();

            if (SubsManager.IsEmpty)
                consumerChannel.CloseAsync().GetAwaiter().GetResult();
        }

        public override async Task Publish(IntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            if (!persistentConnection.IsConnected)
                persistentConnection.TryConnect();

            var policy = Policy.Handle<SocketException>()
                   .Or<BrokerUnreachableException>()
                   .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) => { });

            var eventName = @event.GetType().Name;
            //eventName = ProcessEventName(eventName);

            await consumerChannel.ExchangeDeclareAsync(exchange: EventBusConfig.DefaultTopicName, type: "direct");

            var message = JsonConvert.SerializeObject(@event);

            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(async () =>
            {
                var properties = new BasicProperties();
                properties.DeliveryMode = DeliveryModes.Persistent;

                //await consumerChannel.QueueDeclareAsync(queue: GetSubName(eventName), durable: true, exclusive: false, autoDelete: false, arguments: null);

                await consumerChannel.BasicPublishAsync(exchange: EventBusConfig.DefaultTopicName, routingKey: eventName, mandatory: true, basicProperties: properties, body: body);
            });
        }

        public override async Task Subscribe<T, TH>(CancellationToken cancellationToken = default)
        {
            var eventName = typeof(T).Name;
            //eventName = ProcessEventName(eventName);

            if (!SubsManager.HasSubscriptionForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                    persistentConnection.TryConnect();

                await consumerChannel.QueueDeclareAsync(queue: GetSubName(eventName), durable: true, exclusive: false, autoDelete: false, arguments: null);

                await consumerChannel.QueueBindAsync(queue: GetSubName(eventName), exchange: EventBusConfig.DefaultTopicName, routingKey: eventName);
            }
            SubsManager.AddSubscription<T, TH>();
            await StartBasicConsume(eventName);
        }

        public override Task UnSubscribe<T, TH>(CancellationToken cancellationToken = default)
        {
            SubsManager.RemoveSubscription<T, TH>();

            return Task.CompletedTask;
        }

        private async Task<IChannel> CreateConsumerChannel(CancellationToken cancellationToken = default)
        {
            if (!persistentConnection.IsConnected)
                persistentConnection.TryConnect();

            var channel = await persistentConnection.CreateChannelAsync(cancellationToken);
            await channel.ExchangeDeclareAsync(exchange: EventBusConfig.DefaultTopicName, type: "direct");

            return channel;
        }

        private async Task StartBasicConsume(string eventName)
        {
            if (consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(consumerChannel);
                consumer.ReceivedAsync += Consumer_ReceivedAsync;

                await consumerChannel.BasicConsumeAsync(queue: GetSubName(eventName), autoAck: false, consumer: consumer);
            }
        }

        private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            var eventName = @event.RoutingKey;
            //eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(@event.Body.Span);
            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {

            }
            await consumerChannel.BasicAckAsync(@event.DeliveryTag, multiple: false);
        }
    }
}
