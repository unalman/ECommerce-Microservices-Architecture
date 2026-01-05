using FluentValidation;
using OrderService.Api.Application.Commands;

namespace OrderService.Api.Application.Validations
{
    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator(ILogger<CancelOrderCommandValidator> logger)
        {
            RuleFor(order => order.OrderNumber).NotEmpty().WithMessage("No orderId found");

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
            }
        }
    }
}
