namespace Domain.Orders;

/// <summary>
/// OrderPricingService is a Domain Service that calculates order pricing.
///
/// WHY is this in the Domain layer?
/// ─────────────────────────────────
/// Pricing logic is core business logic - discounts, tax calculations,
/// bulk pricing rules are all business decisions. They should live in the
/// Domain layer where they can be tested independently of any infrastructure.
///
/// In a real application, this might contain complex pricing rules:
///   - Volume discounts
///   - Customer-specific pricing
///   - Promotional pricing
///   - Tax calculations based on jurisdiction
///
/// For this demo, it simply calculates quantity × unit price.
/// </summary>
public class OrderPricingService
{
    /// <summary>
    /// Calculates the total price for an order.
    /// Pure business logic - no external dependencies.
    /// </summary>
    public decimal CalculateTotal(int quantity, decimal unitPrice)
    {
        // Business rule: apply 10% discount for orders of 10 or more items
        decimal total = quantity * unitPrice;

        if (quantity >= 10)
        {
            total *= 0.9m; // 10% bulk discount
        }

        return Math.Round(total, 2);
    }
}
