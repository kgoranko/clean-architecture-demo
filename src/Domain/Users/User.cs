using SharedKernel;

namespace Domain.Users;

public sealed class User : Entity
{
    private User()
    {
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime? WelcomeEmailSentAt { get; private set; }

    public DateTime? DeactivatedAt { get; private set; }

    public string? DeactivationReason { get; private set; }

    public static Result<User> Create(
        Guid id,
        string email,
        string firstName,
        string lastName,
        DateTime createdAt)
    {
        Result<string> emailResult = NormalizeEmail(email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<User>(emailResult.Error);
        }

        Result<(string FirstName, string LastName, string FullName)> nameResult =
            BuildName(firstName, lastName);

        if (nameResult.IsFailure)
        {
            return Result.Failure<User>(nameResult.Error);
        }

        return Result.Success(new User
        {
            Id = id,
            Email = emailResult.Value,
            FirstName = nameResult.Value.FirstName,
            LastName = nameResult.Value.LastName,
            FullName = nameResult.Value.FullName,
            CreatedAt = createdAt,
            IsActive = true
        });
    }

    public static User Rehydrate(
        Guid id,
        string email,
        string firstName,
        string lastName,
        DateTime createdAt,
        bool isActive = true,
        DateTime? welcomeEmailSentAt = null,
        DateTime? deactivatedAt = null,
        string? deactivationReason = null)
    {
        Result<User> userResult = Create(id, email, firstName, lastName, createdAt);

        if (userResult.IsFailure)
        {
            throw new InvalidOperationException(userResult.Error.Description);
        }

        User user = userResult.Value;
        user.IsActive = isActive;
        user.WelcomeEmailSentAt = welcomeEmailSentAt;
        user.DeactivatedAt = deactivatedAt;
        user.DeactivationReason = string.IsNullOrWhiteSpace(deactivationReason)
            ? null
            : deactivationReason.Trim();

        return user;
    }

    public Result ChangeName(string firstName, string lastName)
    {
        Result ensureActiveResult = EnsureActive();
        if (ensureActiveResult.IsFailure)
        {
            return ensureActiveResult;
        }

        Result<(string FirstName, string LastName, string FullName)> nameResult =
            BuildName(firstName, lastName);

        if (nameResult.IsFailure)
        {
            return Result.Failure(nameResult.Error);
        }

        FirstName = nameResult.Value.FirstName;
        LastName = nameResult.Value.LastName;
        FullName = nameResult.Value.FullName;

        return Result.Success();
    }

    public Result ChangeEmail(string email)
    {
        Result ensureActiveResult = EnsureActive();
        if (ensureActiveResult.IsFailure)
        {
            return ensureActiveResult;
        }

        Result<string> emailResult = NormalizeEmail(email);
        if (emailResult.IsFailure)
        {
            return Result.Failure(emailResult.Error);
        }

        Email = emailResult.Value;
        return Result.Success();
    }

    public Result MarkWelcomeEmailSent(DateTime sentAt)
    {
        Result ensureActiveResult = EnsureActive();
        if (ensureActiveResult.IsFailure)
        {
            return ensureActiveResult;
        }

        if (WelcomeEmailSentAt.HasValue)
        {
            return Result.Failure(UserErrors.WelcomeEmailAlreadySent);
        }

        WelcomeEmailSentAt = sentAt;
        return Result.Success();
    }

    public Result Deactivate(DateTime deactivatedAt, string reason)
    {
        if (!IsActive)
        {
            return Result.Failure(UserErrors.UserAlreadyInactive);
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure(UserErrors.DeactivationReasonRequired);
        }

        IsActive = false;
        DeactivatedAt = deactivatedAt;
        DeactivationReason = reason.Trim();

        return Result.Success();
    }

    public Result Reactivate()
    {
        IsActive = true;
        DeactivatedAt = null;
        DeactivationReason = null;

        return Result.Success();
    }

    private Result EnsureActive() =>
        IsActive
            ? Result.Success()
            : Result.Failure(UserErrors.InactiveUserCannotBeModified);

    private static Result<string> NormalizeEmail(string email)
    {
        string normalizedEmail = (email ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalizedEmail) || !normalizedEmail.Contains('@'))
        {
            return Result.Failure<string>(UserErrors.InvalidEmailFormat);
        }

        return Result.Success(normalizedEmail);
    }

    private static Result<(string FirstName, string LastName, string FullName)> BuildName(
        string firstName,
        string lastName)
    {
        string normalizedFirstName = NormalizeNamePart(firstName);
        if (string.IsNullOrWhiteSpace(normalizedFirstName))
        {
            return Result.Failure<(string, string, string)>(UserErrors.InvalidName("First name"));
        }

        string normalizedLastName = NormalizeNamePart(lastName);
        if (string.IsNullOrWhiteSpace(normalizedLastName))
        {
            return Result.Failure<(string, string, string)>(UserErrors.InvalidName("Last name"));
        }

        string fullName = $"{normalizedFirstName} {normalizedLastName}";

        return Result.Success((normalizedFirstName, normalizedLastName, fullName));
    }

    private static string NormalizeNamePart(string value) =>
        string.Join(' ', (value ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}
