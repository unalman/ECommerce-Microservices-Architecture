using MediatR;
using OrderService.Api.Application.Queries;
using OrderService.Api.Infrastructure.Services;

namespace OrderService.Api.Apis
{
    public class OrderServices(
        IMediator mediator,
        IOrderQueries queries,
        IIdentityService identityService,
        ILogger<OrderServices> logger)
    {
        public IMediator Mediator { get; set; } = mediator;
        public IOrderQueries Queries { get; set; } = queries;
        public IIdentityService IdentityService { get; set; } = identityService;
        public ILogger<OrderServices> Logger { get; set; } = logger;
    }
}
