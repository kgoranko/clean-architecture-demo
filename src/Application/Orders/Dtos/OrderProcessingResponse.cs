namespace Application.Orders.Dtos;

public sealed record OrderProcessingResponse(
    string OrderId,
    string Status,
    string Message,
    decimal TotalPrice);
