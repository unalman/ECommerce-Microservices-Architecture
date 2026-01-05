using Microsoft.Extensions.DependencyInjection;
namespace EventBus.Base.Abstraction
{
    public interface IEventBusBuilder
    {
        public IServiceCollection Services { get; }
    }
}
