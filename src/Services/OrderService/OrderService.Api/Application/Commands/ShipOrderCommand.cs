using MediatR;

namespace OrderService.Api.Application.Commands
{
    public record ShipOrderCommand(int OrderNumber) : IRequest<bool>;
}
