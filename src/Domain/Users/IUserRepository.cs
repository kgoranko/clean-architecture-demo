namespace Domain.Users;

/// <summary>
/// IUserRepository is a Domain Interface (Port) for user data access.
///
/// WHY is this in the Domain layer?
/// ─────────────────────────────────
/// Same principle as IOrderRepository - the Domain defines the contract,
/// Infrastructure provides the implementation. This is the Repository Pattern
/// combined with Dependency Inversion.
///
/// The Application layer will depend on this interface (which is fine - Application
/// can depend on Domain). The Infrastructure layer will implement it.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task SaveAsync(User user, CancellationToken cancellationToken = default);
}
