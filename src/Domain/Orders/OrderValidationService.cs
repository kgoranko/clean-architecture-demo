using SharedKernel;

namespace Domain.Orders;

/// <summary>
/// OrderValidationService is a Domain Service - it encapsulates business rules
/// that don't naturally belong to a single entity.
///
/// WHY is this in the Domain layer?
/// ─────────────────────────────────
/// Business validation rules (e.g., "stock must be sufficient", "product must exist")
/// are part of the core business logic. They don't depend on any infrastructure
/// (no database calls, no HTTP requests). The service receives data and applies
/// pure business rules to produce a Result.
///
/// Domain Services should be:
///   - Stateless (no mutable fields)
///   - Free of infrastructure dependencies
///   - Focused on business logic that spans multiple entities or concepts
/// </summary>
public class OrderValidationService
{
    /// <summary>
    /// Validates that the product exists and there is sufficient stock.
    /// This is a pure business rule - no infrastructure dependency needed.
    /// </summary>
    public Result ValidateOrder(bool productExists, int availableStock, string productName, int requestedQuantity)
    {
        // Step 1: Check if the product exists in our catalog
        if (!productExists)
        {
            return Result.Failure(OrderErrors.ProductNotFound(productName));
        }

        // Step 2: Check if we have enough stock
        if (availableStock < requestedQuantity)
        {
            return Result.Failure(OrderErrors.InsufficientStock(productName, requestedQuantity, availableStock));
        }

        return Result.Success();
    }
}
