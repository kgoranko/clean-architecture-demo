using Application.Abstractions.Notifications;
using Domain.Users;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.Events;

internal sealed class SendWelcomeEmailOnUserCreatedDomainEventHandler(
    IEmailProvider emailProvider,
    IUserRepository userRepository,
    IDateTimeProvider dateTimeProvider,
    ILogger<SendWelcomeEmailOnUserCreatedDomainEventHandler> logger)
    : IDomainEventHandler<EntityCreatedDomainEvent<User>>
{
    public async Task Handle(EntityCreatedDomainEvent<User> domainEvent, CancellationToken cancellationToken)
    {
        User createdUser = domainEvent.Entity;

        if (createdUser.WelcomeEmailSentAt.HasValue)
        {
            return;
        }

        Result emailResult = await emailProvider.SendWelcomeEmailAsync(
            createdUser.Email,
            createdUser.FirstName,
            cancellationToken);

        if (emailResult.IsFailure)
        {
            logger.LogWarning(
                "Welcome email failed for user {UserId}: {Error}",
                createdUser.Id,
                emailResult.Error.Description);

            return;
        }

        User? user = await userRepository.GetByEmailAsync(createdUser.Email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {UserId} was not found after EntityCreatedDomainEvent<User>", createdUser.Id);
            return;
        }

        Result markEmailResult = user.MarkWelcomeEmailSent(dateTimeProvider.UtcNow);
        if (markEmailResult.IsFailure)
        {
            logger.LogWarning(
                "Could not mark welcome email as sent for user {UserId}: {Error}",
                createdUser.Id,
                markEmailResult.Error.Description);

            return;
        }

        await userRepository.SaveAsync(user, cancellationToken);
    }
}
