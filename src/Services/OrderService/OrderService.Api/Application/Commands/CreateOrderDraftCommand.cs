using MediatR;
using OrderService.Api.Application.Models;

namespace OrderService.Api.Application.Commands
{
    public record CreateOrderDraftCommand(string BuyerId, IEnumerable<BasketItem> Items):IRequest<OrderDraftDTO>;
}
