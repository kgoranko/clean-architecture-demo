using Application.Abstractions.Messaging;
using Application.Abstractions.Notifications;
using Domain.Users;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Users.RegisterUser;

/// <summary>
/// Orchestrates the user registration use case after the command pipeline
/// has completed validation and entered the handler.
/// </summary>
internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IEmailProviderFailureSimulation emailFailureSimulation,
    IDateTimeProvider dateTimeProvider,
    ILogger<RegisterUserCommandHandler> logger) : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly UserValidationService _validationService = new();

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting user registration for {Email}", command.Email);

        bool emailExists = await userRepository.EmailExistsAsync(command.Email, cancellationToken);
        if (emailExists)
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        Result domainValidation = _validationService.ValidateEmailDomain(command.Email);
        if (domainValidation.IsFailure)
        {
            return Result.Failure<Guid>(domainValidation.Error);
        }

        Result<User> userResult = User.Create(
            Guid.NewGuid(),
            command.Email,
            command.FirstName,
            command.LastName,
            dateTimeProvider.UtcNow);

        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        User user = userResult.Value;

        string normalizedEmailTrigger = (command.EmailTrigger ?? string.Empty).Trim();
        using IDisposable? emailFailureScope = string.Equals(normalizedEmailTrigger, "fail", StringComparison.OrdinalIgnoreCase)
            ? emailFailureSimulation.FailWelcomeEmail()
            : null;

        await userRepository.SaveAsync(user, cancellationToken);
        logger.LogInformation("User {UserId} ({FullName}) saved to database", user.Id, user.FullName);

        if (string.Equals(normalizedEmailTrigger, "fail", StringComparison.OrdinalIgnoreCase)
            && emailFailureSimulation.WelcomeEmailFailed)
        {
            logger.LogError("Email provider unavailable during registration for {Email}", command.Email);
            return Result.Failure<Guid>(UserErrors.EmailProviderUnavailable);
        }

        logger.LogInformation("User {UserId} registered successfully", user.Id);

        return Result.Success(user.Id);
    }
}
