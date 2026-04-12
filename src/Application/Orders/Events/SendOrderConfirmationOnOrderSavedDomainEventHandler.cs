using Application.Abstractions.Notifications;
using Domain.Orders.Events;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Orders.Events;

internal sealed class SendOrderConfirmationOnOrderSavedDomainEventHandler(
    IEmailProvider emailProvider,
    ILogger<SendOrderConfirmationOnOrderSavedDomainEventHandler> logger)
    : IDomainEventHandler<OrderSavedDomainEvent>
{
    public async Task Handle(OrderSavedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result result = await emailProvider.SendOrderConfirmationAsync(
            domainEvent.CustomerName,
            domainEvent.OrderId.ToString(),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Order confirmation email failed for order {OrderId}: {Error}",
                domainEvent.OrderId,
                result.Error.Description);
        }
    }
}
