using SharedKernel;

namespace Infrastructure.DomainEvents;

internal interface IDomainEventsDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
