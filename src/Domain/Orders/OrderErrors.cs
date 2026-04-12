using SharedKernel;

namespace Domain.Orders;

/// <summary>
/// OrderErrors defines all possible error states for Order operations.
/// These live in the Domain layer because they represent business-level errors
/// (product not found, insufficient stock) - not technical errors.
/// </summary>
public static class OrderErrors
{
    public static readonly Error InvalidCustomerName = Error.Problem(
        "Orders.InvalidCustomerName",
        "Customer name is required.");

    public static readonly Error InvalidProductName = Error.Problem(
        "Orders.InvalidProductName",
        "Product name is required.");

    public static Error ProductNotFound(string productName) => Error.NotFound(
        "Orders.ProductNotFound",
        $"The product '{productName}' was not found in the catalog.");

    public static Error InsufficientStock(string productName, int requested, int available) => Error.Problem(
        "Orders.InsufficientStock",
        $"Insufficient stock for '{productName}'. Requested: {requested}, Available: {available}.");

    public static Error PaymentDeclined(string reason) => Error.Failure(
        "Orders.PaymentDeclined",
        $"Payment was declined: {reason}.");

    public static Error InvalidQuantity(int quantity) => Error.Problem(
        "Orders.InvalidQuantity",
        $"Quantity must be greater than zero. Current value: {quantity}.");

    public static Error InvalidUnitPrice(decimal unitPrice) => Error.Problem(
        "Orders.InvalidUnitPrice",
        $"Unit price must be greater than zero. Current value: {unitPrice}.");

    public static Error InvalidTotalPrice(decimal totalPrice) => Error.Problem(
        "Orders.InvalidTotalPrice",
        $"Total price must be greater than zero. Current value: {totalPrice}.");

    public static readonly Error OrderAlreadyProcessed = Error.Conflict(
        "Orders.OrderAlreadyProcessed",
        "The order has already been processed.");

    public static readonly Error CancelledOrderCannotBeConfirmed = Error.Conflict(
        "Orders.CancelledOrderCannotBeConfirmed",
        "A cancelled order cannot be confirmed.");

    public static readonly Error OnlyPendingOrdersCanBeModified = Error.Conflict(
        "Orders.OnlyPendingOrdersCanBeModified",
        "Only pending orders can be modified.");

    public static readonly Error OrderAlreadyCancelled = Error.Conflict(
        "Orders.OrderAlreadyCancelled",
        "The order has already been cancelled.");

    public static readonly Error ShippedOrderCannotBeCancelled = Error.Conflict(
        "Orders.ShippedOrderCannotBeCancelled",
        "A shipped order cannot be cancelled.");

    public static readonly Error CancellationReasonRequired = Error.Problem(
        "Orders.CancellationReasonRequired",
        "A cancellation reason is required.");

    public static readonly Error OrderMustBeConfirmedBeforeShipping = Error.Conflict(
        "Orders.OrderMustBeConfirmedBeforeShipping",
        "Only confirmed orders can be marked as shipped.");
}
