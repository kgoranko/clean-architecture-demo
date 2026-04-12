using Domain.Orders;
using Domain.Users;
using Infrastructure.DomainEvents;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedKernel;

namespace Infrastructure.Persistence;

internal sealed class DemoDbContext(
    DbContextOptions<DemoDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    public DbSet<User> Users => Set<User>();

    public DbSet<ProductCatalogItem> ProductCatalog => Set<ProductCatalogItem>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        RaiseGenericLifecycleDomainEvents();

        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCatalogItemConfiguration());
    }

    private void RaiseGenericLifecycleDomainEvents()
    {
        foreach (EntityEntry<Entity> entry in ChangeTracker.Entries<Entity>()
            .Where(ShouldRaiseGenericLifecycleDomainEvent))
        {
            entry.Entity.Raise(CreateGenericLifecycleDomainEvent(entry));
        }
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> events = entity.DomainEvents;
                entity.ClearDomainEvents();

                return events;
            })
            .ToList();

        await domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);
    }

    private static bool ShouldRaiseGenericLifecycleDomainEvent(EntityEntry<Entity> entry) =>
        entry.Entity is User &&
        entry.State is EntityState.Added or EntityState.Modified;

    private static IDomainEvent CreateGenericLifecycleDomainEvent(EntityEntry<Entity> entry)
    {
        Type eventDefinition = entry.State switch
        {
            EntityState.Added => typeof(EntityCreatedDomainEvent<>),
            EntityState.Modified => typeof(EntityUpdatedDomainEvent<>),
            _ => throw new InvalidOperationException($"Unsupported entity state {entry.State} for lifecycle event creation.")
        };

        Type eventType = eventDefinition.MakeGenericType(entry.Entity.GetType());

        return (IDomainEvent)Activator.CreateInstance(eventType, entry.Entity)!;
    }
}
