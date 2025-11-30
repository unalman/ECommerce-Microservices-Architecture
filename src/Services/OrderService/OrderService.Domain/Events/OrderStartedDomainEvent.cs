using MediatR;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Domain.Events;

public record OrderStartedDomainEvent(
    Order Order,
    string UserId,
    string UserName,
    int CardTypeId,
    string CardNumber,
    string CardSecurityNumber,
    string CardHolderName,
    DateTime CardExpiration) : INotification;

