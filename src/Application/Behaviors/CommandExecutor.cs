using Application.Abstractions.Behaviors;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Behaviors;

public sealed class CommandExecutor<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> _handler;
    private readonly IEnumerable<ICommandBehavior<TCommand, TResponse>> _behaviors;

    public CommandExecutor(
        ICommandHandler<TCommand, TResponse> handler,
        IEnumerable<ICommandBehavior<TCommand, TResponse>> behaviors)
    {
        _handler = handler;
        _behaviors = behaviors;
    }

    public Task<Result<TResponse>> Execute(
        TCommand command,
        CancellationToken cancellationToken = default)
    {
        Func<Task<Result<TResponse>>> next =
            () => _handler.Handle(command, cancellationToken);

        foreach (ICommandBehavior<TCommand, TResponse> behavior in _behaviors.Reverse())
        {
            Func<Task<Result<TResponse>>> current = next;
            next = () => behavior.Handle(command, current, cancellationToken);
        }

        return next();
    }
}
