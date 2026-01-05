using MediatR;

namespace OrderService.Api.Application.Commands
{
    public record SetAwaitingValidationOrderStatusCommand(int OrderNumber) : IRequest<bool>;
}
