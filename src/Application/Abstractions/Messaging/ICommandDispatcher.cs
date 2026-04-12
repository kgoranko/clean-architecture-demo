using SharedKernel;

namespace Application.Abstractions.Messaging;

public interface ICommandDispatcher
{
    Task<Result<TResponse>> Send<TCommand, TResponse>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>;
}
