using EventBus.Base.Abstraction;
using EventBus.Base.Extensions;
using MediatR;
using OrderService.Api.Application.Commands;
using OrderService.Api.Application.IntegrationEvents.Events;

namespace OrderService.Api.Application.IntegrationEvents.EventHandling
{
    public class OrderStockRejectedIntegrationEventHandler(
        IMediator mediator,
        ILogger<OrderStockRejectedIntegrationEventHandler> logger)
        : IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>
    {
        public async Task Handle(OrderStockRejectedIntegrationEvent @event)
        {
            logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

            var orderStockRejectedItems = @event.OrderStockItems
                .FindAll(x => !x.HasStock)
                .Select(x => x.ProductId)
                .ToList();

            var command = new SetStockRejectedOrderStatusCommand(@event.OrderId, orderStockRejectedItems);

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
