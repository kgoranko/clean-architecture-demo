namespace SharedKernel;

public sealed record EntityCreatedDomainEvent<TEntity>(TEntity Entity) : IDomainEvent
    where TEntity : Entity;
