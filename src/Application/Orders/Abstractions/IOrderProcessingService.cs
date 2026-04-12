using Application.Orders.Dtos;
using SharedKernel;

namespace Application.Orders.Abstractions;

/// <summary>
/// IOrderProcessingService is an Application Service interface.
///
/// This demonstrates the FIRST approach to Clean Architecture:
/// ═══════════════════════════════════════════════════════════
/// DIRECT DI SERVICE CALL
///
/// The Razor Page (Presentation layer) injects this interface and calls
/// the method directly. The implementation orchestrates the business process
/// by calling Domain services and Infrastructure services.
///
/// Flow: UI → Application Service → Domain Services + Infrastructure Providers
///
/// Pros:
///   - Simple and straightforward
///   - Easy to understand and debug
///   - Good for smaller projects or simple operations
///
/// Cons:
///   - Harder to add cross-cutting concerns (logging, validation)
///   - Less decoupled than CQRS
///   - Can grow into large "god services" if not careful
/// </summary>
public interface IOrderProcessingService
{
    Task<Result<OrderProcessingResponse>> ProcessOrderAsync(
        OrderProcessingRequest request,
        CancellationToken cancellationToken = default);
}
