using MediatR;

namespace OrderService.Api.Application.Commands
{
    public record CancelOrderCommand(int OrderNumber) : IRequest<bool>;
}
