using Domain.Orders.Events;
using SharedKernel;

namespace Domain.Orders;

public sealed class Order : Entity
{
    public const string PendingStatus = "Pending";
    public const string ConfirmedStatus = "Confirmed";
    public const string CancelledStatus = "Cancelled";
    public const string ShippedStatus = "Shipped";

    private Order()
    {
    }

    public Guid Id { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal TotalPrice { get; private set; }

    public string Status { get; private set; } = PendingStatus;

    public DateTime CreatedAt { get; private set; }

    public DateTime? ProcessedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }

    public DateTime? ShippedAt { get; private set; }

    public static Result<Order> Create(
        Guid id,
        string customerName,
        string productName,
        int quantity,
        decimal unitPrice,
        decimal totalPrice,
        DateTime createdAt)
    {
        string normalizedCustomerName = NormalizeName(customerName);
        if (string.IsNullOrWhiteSpace(normalizedCustomerName))
        {
            return Result.Failure<Order>(OrderErrors.InvalidCustomerName);
        }

        string normalizedProductName = NormalizeName(productName);
        if (string.IsNullOrWhiteSpace(normalizedProductName))
        {
            return Result.Failure<Order>(OrderErrors.InvalidProductName);
        }

        if (quantity <= 0)
        {
            return Result.Failure<Order>(OrderErrors.InvalidQuantity(quantity));
        }

        if (unitPrice <= 0)
        {
            return Result.Failure<Order>(OrderErrors.InvalidUnitPrice(unitPrice));
        }

        if (totalPrice <= 0)
        {
            return Result.Failure<Order>(OrderErrors.InvalidTotalPrice(totalPrice));
        }

        return Result.Success(new Order
        {
            Id = id,
            CustomerName = normalizedCustomerName,
            ProductName = normalizedProductName,
            Quantity = quantity,
            UnitPrice = decimal.Round(unitPrice, 2),
            TotalPrice = decimal.Round(totalPrice, 2),
            Status = PendingStatus,
            CreatedAt = createdAt
        });
    }

    public static Order Rehydrate(
        Guid id,
        string customerName,
        string productName,
        int quantity,
        decimal unitPrice,
        decimal totalPrice,
        string status,
        DateTime createdAt,
        DateTime? processedAt = null,
        DateTime? cancelledAt = null,
        string? cancellationReason = null,
        DateTime? shippedAt = null)
    {
        Result<Order> orderResult = Create(
            id,
            customerName,
            productName,
            quantity,
            unitPrice,
            totalPrice,
            createdAt);

        if (orderResult.IsFailure)
        {
            throw new InvalidOperationException(orderResult.Error.Description);
        }

        Order order = orderResult.Value;
        order.Status = string.IsNullOrWhiteSpace(status) ? PendingStatus : status.Trim();
        order.ProcessedAt = processedAt;
        order.CancelledAt = cancelledAt;
        order.CancellationReason = string.IsNullOrWhiteSpace(cancellationReason)
            ? null
            : cancellationReason.Trim();
        order.ShippedAt = shippedAt;

        return order;
    }

    public Result Confirm(DateTime processedAt)
    {
        if (Status == CancelledStatus)
        {
            return Result.Failure(OrderErrors.CancelledOrderCannotBeConfirmed);
        }

        if (Status == ConfirmedStatus || Status == ShippedStatus)
        {
            return Result.Failure(OrderErrors.OrderAlreadyProcessed);
        }

        Status = ConfirmedStatus;
        ProcessedAt = processedAt;

        return Result.Success();
    }

    public Result AdjustQuantity(int quantity, decimal recalculatedTotal)
    {
        if (Status != PendingStatus)
        {
            return Result.Failure(OrderErrors.OnlyPendingOrdersCanBeModified);
        }

        if (quantity <= 0)
        {
            return Result.Failure(OrderErrors.InvalidQuantity(quantity));
        }

        if (recalculatedTotal <= 0)
        {
            return Result.Failure(OrderErrors.InvalidTotalPrice(recalculatedTotal));
        }

        Quantity = quantity;
        TotalPrice = decimal.Round(recalculatedTotal, 2);

        return Result.Success();
    }

    public Result Cancel(string reason, DateTime cancelledAt)
    {
        if (Status == CancelledStatus)
        {
            return Result.Failure(OrderErrors.OrderAlreadyCancelled);
        }

        if (Status == ShippedStatus)
        {
            return Result.Failure(OrderErrors.ShippedOrderCannotBeCancelled);
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure(OrderErrors.CancellationReasonRequired);
        }

        Status = CancelledStatus;
        CancelledAt = cancelledAt;
        CancellationReason = reason.Trim();

        return Result.Success();
    }

    public Result MarkAsShipped(DateTime shippedAt)
    {
        if (Status != ConfirmedStatus)
        {
            return Result.Failure(OrderErrors.OrderMustBeConfirmedBeforeShipping);
        }

        Status = ShippedStatus;
        ShippedAt = shippedAt;

        return Result.Success();
    }

    public void RaiseSavedDomainEvent() =>
        Raise(new OrderSavedDomainEvent(Id, CustomerName));

    private static string NormalizeName(string value) =>
        string.Join(' ', (value ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}
