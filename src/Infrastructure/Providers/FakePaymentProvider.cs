using Application.Abstractions.Payments;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.DependencyInjection;

namespace Infrastructure.Providers;

internal sealed class FakePaymentProvider(ILogger<FakePaymentProvider> logger)
    : IPaymentProvider, IScopedService
{
    public Task<Result> ProcessPaymentAsync(
        decimal amount,
        string customerName,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[PAYMENT PROVIDER] Processing payment of {Amount:C} for {Customer}",
            amount,
            customerName);

        return Task.FromResult(Result.Success());
    }
}
