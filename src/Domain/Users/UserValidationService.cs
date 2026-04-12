using SharedKernel;

namespace Domain.Users;

/// <summary>
/// UserValidationService is a Domain Service for user-related business rules.
///
/// WHY is this in the Domain layer?
/// ─────────────────────────────────
/// Email domain validation is a business rule - the company decides which
/// email domains are allowed for registration. This is not a technical
/// validation (format checking), but a business policy decision.
///
/// Domain Services encapsulate business rules that:
///   - Don't naturally fit inside a single entity
///   - Need to be shared across multiple use cases
///   - Represent core business policies
/// </summary>
public class UserValidationService
{
    // Business rule: only these email domains are allowed
    private static readonly string[] AllowedDomains = ["company.com", "partner.com", "gmail.com", "outlook.com"];

    /// <summary>
    /// Validates that the email domain is allowed by business policy.
    /// </summary>
    public Result ValidateEmailDomain(string email)
    {
        string[] parts = email.Split('@');
        string domain = parts[^1];

        // Business rule: check if the email domain is in our allowed list
        if (!AllowedDomains.Contains(domain, StringComparer.OrdinalIgnoreCase))
        {
            return Result.Failure(UserErrors.InvalidEmailDomain(domain));
        }

        return Result.Success();
    }
}
