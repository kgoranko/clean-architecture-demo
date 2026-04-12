using SharedKernel;

namespace Domain.Orders.Events;

public sealed record OrderSavedDomainEvent(
    Guid OrderId,
    string CustomerName) : IDomainEvent;
