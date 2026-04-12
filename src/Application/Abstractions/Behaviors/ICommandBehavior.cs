using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Abstractions.Behaviors;

public interface ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(
        TCommand command,
        Func<Task<Result<TResponse>>> next,
        CancellationToken cancellationToken);
}
