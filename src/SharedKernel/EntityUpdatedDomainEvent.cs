namespace SharedKernel;

public sealed record EntityUpdatedDomainEvent<TEntity>(TEntity Entity) : IDomainEvent
    where TEntity : Entity;
