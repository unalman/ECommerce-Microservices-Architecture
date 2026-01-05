using MediatR;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Idempotency;

namespace OrderService.Api.Application.Commands
{
    public class SetStockConfirmedOrderStatusCommandHandler : IRequestHandler<SetStockConfirmedOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public SetStockConfirmedOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Handler which processes the command when
        /// Stock service confirms the request
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(SetStockConfirmedOrderStatusCommand command, CancellationToken cancellationToken)
        {
            // Simulate a work time for confirming the stock
            await Task.Delay(10000, cancellationToken);

            var orderToUpdateStock = await _orderRepository.GetAsync(command.OrderNumber);
            if (orderToUpdateStock == null)
            {
                return false;
            }

            orderToUpdateStock.SetStockConfirmedStatus();
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }

    // Use for Idempotency in Command process
    public class SetStockConfirmedOrderStatusIdentifiedCommandHandler : IdentifiedCommandHandler<SetStockConfirmedOrderStatusCommand, bool>
    {
        public SetStockConfirmedOrderStatusIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<SetStockConfirmedOrderStatusCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true; // Ignore duplicate requests for processing order.
        }
    }
}
