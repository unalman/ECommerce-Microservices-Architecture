using MediatR;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Api.Application.Commands
{
    public class SetPaidOrderStatusCommandHandler:IRequestHandler<SetPaidOrderStatusCommand,bool>
    {
        private readonly IOrderRepository _orderRepository;
        public SetPaidOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Handler which processes the command when
        /// Shipment service confirms the payment
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(SetPaidOrderStatusCommand command, CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken);

            var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
            if (orderToUpdate == null)
            {
                return false;
            }

            orderToUpdate.SetPaidStatus();
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
