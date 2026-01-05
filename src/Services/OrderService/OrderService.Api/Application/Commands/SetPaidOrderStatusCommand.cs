using MediatR;

namespace OrderService.Api.Application.Commands
{
    public record SetPaidOrderStatusCommand(int OrderNumber) : IRequest<bool>;
}
