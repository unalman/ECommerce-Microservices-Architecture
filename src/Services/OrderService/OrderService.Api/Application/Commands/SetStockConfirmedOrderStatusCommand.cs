using MediatR;

namespace OrderService.Api.Application.Commands
{
    public record SetStockConfirmedOrderStatusCommand(int OrderNumber) : IRequest<bool>;
}
