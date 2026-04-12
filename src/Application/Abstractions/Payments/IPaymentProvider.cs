using SharedKernel;

namespace Application.Abstractions.Payments;

public interface IPaymentProvider
{
    Task<Result> ProcessPaymentAsync(
        decimal amount,
        string customerName,
        CancellationToken cancellationToken = default);
}
