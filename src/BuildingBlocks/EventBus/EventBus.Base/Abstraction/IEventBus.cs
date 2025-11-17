
using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBus : IDisposable
    {
        Task Publish(IntegrationEvent @event, CancellationToken cancellationToken = default);
        Task Subscribe<T, TH>(CancellationToken cancellationToken = default) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        Task UnSubscribe<T, TH>(CancellationToken cancellationToken = default) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
