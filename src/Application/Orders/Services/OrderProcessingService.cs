using Application.Abstractions.Payments;
using Application.Orders.Abstractions;
using Application.Orders.Dtos;
using Domain.Orders;
using Domain.Orders.Events;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.DependencyInjection;

namespace Application.Orders.Services;

/// <summary>
/// Application-layer orchestrator for the order processing use case.
/// </summary>
internal sealed class OrderProcessingService(
    IOrderRepository orderRepository,
    IPaymentProvider paymentProvider,
    IEnumerable<IDomainEventHandler<OrderSavedDomainEvent>> orderSavedDomainEventHandlers,
    IDateTimeProvider dateTimeProvider,
    ILogger<OrderProcessingService> logger) : IOrderProcessingService, IScopedService
{
    private readonly OrderValidationService _validationService = new();
    private readonly OrderPricingService _pricingService = new();

    public async Task<Result<OrderProcessingResponse>> ProcessOrderAsync(
        OrderProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Processing order for {Customer}, Product: {Product}",
            request.CustomerName,
            request.ProductName);

        // Step 1: ask Infrastructure whether the selected product exists.
        bool productExists = await orderRepository.ProductExistsAsync(request.ProductName, cancellationToken);

        // Step 2: load the current stock level from Infrastructure.
        int availableStock = await orderRepository.GetStockQuantityAsync(request.ProductName, cancellationToken);

        // Step 3: let the Domain decide whether the request is valid.
        Result validationResult = _validationService.ValidateOrder(
            productExists,
            availableStock,
            request.ProductName,
            request.Quantity);

        if (validationResult.IsFailure)
        {
            return Result.Failure<OrderProcessingResponse>(validationResult.Error);
        }

        // Step 4: read the catalog price and let the Domain calculate the total.
        decimal? unitPrice = await orderRepository.GetUnitPriceAsync(request.ProductName, cancellationToken);
        if (unitPrice is null)
        {
            return Result.Failure<OrderProcessingResponse>(OrderErrors.ProductNotFound(request.ProductName));
        }

        decimal totalPrice = _pricingService.CalculateTotal(request.Quantity, unitPrice.Value);

        // Step 5: create the aggregate so the entity can enforce its invariants.
        Result<Order> orderResult = Order.Create(
            Guid.NewGuid(),
            request.CustomerName,
            request.ProductName,
            request.Quantity,
            unitPrice.Value,
            totalPrice,
            dateTimeProvider.UtcNow);

        if (orderResult.IsFailure)
        {
            return Result.Failure<OrderProcessingResponse>(orderResult.Error);
        }

        Order order = orderResult.Value;

        // Step 6: call the external payment provider before the order is confirmed.
        string normalizedPaymentTrigger = (request.PaymentTrigger ?? string.Empty).Trim();
        if (string.Equals(normalizedPaymentTrigger, "decline", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<OrderProcessingResponse>(
                OrderErrors.PaymentDeclined("Card declined by issuing bank."));
        }

        await paymentProvider.ProcessPaymentAsync(totalPrice, request.CustomerName, cancellationToken);

        // Step 7: let the aggregate perform the state transition.
        Result confirmOrderResult = order.Confirm(dateTimeProvider.UtcNow);
        if (confirmOrderResult.IsFailure)
        {
            return Result.Failure<OrderProcessingResponse>(confirmOrderResult.Error);
        }

        // Step 8: persist the confirmed aggregate, then raise the custom order event.
        await orderRepository.SaveAsync(order, cancellationToken);
        order.RaiseSavedDomainEvent();

        // Step 9: dispatch the custom order event so infrastructure handlers can react.
        await DispatchOrderSavedDomainEventsAsync(order, cancellationToken);

        logger.LogInformation(
            "Order {OrderId} processed successfully for {Customer}",
            order.Id,
            request.CustomerName);

        return Result.Success(new OrderProcessingResponse(
            order.Id.ToString(),
            order.Status,
            $"Order for {request.Quantity}x {request.ProductName} has been confirmed!",
            totalPrice));
    }

    private async Task DispatchOrderSavedDomainEventsAsync(Order order, CancellationToken cancellationToken)
    {
        OrderSavedDomainEvent[] orderSavedEvents = order.DomainEvents
            .OfType<OrderSavedDomainEvent>()
            .ToArray();

        foreach (OrderSavedDomainEvent domainEvent in orderSavedEvents)
        {
            foreach (IDomainEventHandler<OrderSavedDomainEvent> handler in orderSavedDomainEventHandlers)
            {
                await handler.Handle(domainEvent, cancellationToken);
            }
        }

        order.ClearDomainEvents();
    }
}
