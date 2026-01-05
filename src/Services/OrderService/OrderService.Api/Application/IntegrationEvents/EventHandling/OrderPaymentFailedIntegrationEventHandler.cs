using EventBus.Base.Abstraction;
using EventBus.Base.Extensions;
using MediatR;
using OrderService.Api.Application.Commands;
using OrderService.Api.Application.IntegrationEvents.Events;

namespace OrderService.Api.Application.IntegrationEvents.EventHandling
{
    public class OrderPaymentFailedIntegrationEventHandler(
        IMediator mediator,
        ILogger<OrderPaymentFailedIntegrationEventHandler> logger) 
        : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

            var command = new CancelOrderCommand(@event.OrderId);

            logger.LogInformation(
                 "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                 command.GetGenericTypeName(),
                 nameof(command.OrderNumber),
                 command.OrderNumber,
                 command);

            await mediator.Send(command);
        }
    }
}
