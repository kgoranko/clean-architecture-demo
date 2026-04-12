namespace Domain.Orders;

/// <summary>
/// IOrderRepository is a Domain Interface (Port) - it defines WHAT data access
/// operations the domain needs, but NOT HOW they are implemented.
///
/// WHY is this in the Domain layer?
/// ─────────────────────────────────
/// This is the "Dependency Inversion Principle" in action. The Domain layer
/// defines the interface (the contract/port), and the Infrastructure layer
/// provides the implementation (the adapter). This way:
///   - Domain never depends on Infrastructure
///   - Infrastructure depends on Domain (it implements the interface)
///   - The flow of control is inverted compared to traditional layered architecture
///
/// In a real application, the implementation might use EF Core, Dapper, or any
/// other data access technology - but the Domain doesn't know or care about that.
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ProductExistsAsync(string productName, CancellationToken cancellationToken = default);
    Task<int> GetStockQuantityAsync(string productName, CancellationToken cancellationToken = default);
    Task<decimal?> GetUnitPriceAsync(string productName, CancellationToken cancellationToken = default);
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);
}
