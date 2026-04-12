using Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.DependencyInjection;

namespace Infrastructure.Providers;

internal sealed class FakeEmailProvider(
    IEmailProviderFailureSimulation failureSimulation,
    ILogger<FakeEmailProvider> logger) : IEmailProvider, IScopedService
{
    public Task<Result> SendOrderConfirmationAsync(
        string customerName,
        string orderId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[EMAIL PROVIDER] Sending order confirmation to {Customer} for order {OrderId}",
            customerName,
            orderId);

        return Task.FromResult(Result.Success());
    }

    public Task<Result> SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default)
    {
        if (failureSimulation.ShouldFailWelcomeEmail)
        {
            logger.LogWarning(
                "[EMAIL PROVIDER] Welcome email provider unavailable for {Email}",
                email);

            failureSimulation.RecordWelcomeEmailFailure();

            return Task.FromResult(Result.Failure(Error.Failure(
                "EmailProvider.WelcomeEmailUnavailable",
                "The welcome email provider is currently unavailable.")));
        }

        logger.LogInformation(
            "[EMAIL PROVIDER] Sending welcome email to {Email} for {FirstName}",
            email,
            firstName);

        return Task.FromResult(Result.Success());
    }
}
