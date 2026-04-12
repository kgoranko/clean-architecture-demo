namespace Application.Orders.Dtos;

public sealed record OrderProcessingRequest(
    string CustomerName,
    string ProductName,
    int Quantity,
    string PaymentTrigger);
