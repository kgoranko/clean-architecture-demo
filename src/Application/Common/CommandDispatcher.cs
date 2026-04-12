using Application.Abstractions.Messaging;
using Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;
using SharedKernel.DependencyInjection;

namespace Application.Common;

public sealed class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher, IScopedService
{
    public Task<Result<TResponse>> Send<TCommand, TResponse>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>
    {
        CommandExecutor<TCommand, TResponse> executor =
            serviceProvider.GetRequiredService<CommandExecutor<TCommand, TResponse>>();

        return executor.Execute(command, cancellationToken);
    }
}
