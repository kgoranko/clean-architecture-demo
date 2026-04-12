using SharedKernel;

namespace Domain.Users;

/// <summary>
/// UserErrors defines all possible error states for User operations.
/// Business-level errors live in the Domain layer.
/// </summary>
public static class UserErrors
{
    public static Error InvalidName(string fieldName) => Error.Problem(
        "Users.InvalidName",
        $"{fieldName} is required.");

    public static readonly Error InvalidEmailFormat = Error.Problem(
        "Users.InvalidEmailFormat",
        "The provided email address is not valid.");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email address is already in use.");

    public static Error InvalidEmailDomain(string domain) => Error.Problem(
        "Users.InvalidEmailDomain",
        $"The email domain '{domain}' is not allowed for registration.");

    public static readonly Error EmailProviderUnavailable = Error.Failure(
        "Users.EmailProviderUnavailable",
        "The user was saved, but the welcome email provider is currently unavailable.");

    public static readonly Error WelcomeEmailAlreadySent = Error.Conflict(
        "Users.WelcomeEmailAlreadySent",
        "The welcome email has already been sent.");

    public static readonly Error UserAlreadyInactive = Error.Conflict(
        "Users.UserAlreadyInactive",
        "The user is already inactive.");

    public static readonly Error InactiveUserCannotBeModified = Error.Conflict(
        "Users.InactiveUserCannotBeModified",
        "Inactive users cannot be modified.");

    public static readonly Error DeactivationReasonRequired = Error.Problem(
        "Users.DeactivationReasonRequired",
        "A deactivation reason is required.");
}
