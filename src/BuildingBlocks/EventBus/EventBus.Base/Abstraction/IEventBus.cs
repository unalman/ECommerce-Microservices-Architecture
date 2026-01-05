
using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBus : IDisposable
    {
        Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default);
    }
}
