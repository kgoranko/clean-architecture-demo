using SharedKernel;

namespace Application.Abstractions.Notifications;

public interface IEmailProvider
{
    Task<Result> SendOrderConfirmationAsync(
        string customerName,
        string orderId,
        CancellationToken cancellationToken = default);

    Task<Result> SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default);
}
