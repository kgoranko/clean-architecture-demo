using Application.Abstractions.Behaviors;
using Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel;

namespace Application.Behaviors;

internal sealed class LoggingBehavior<TCommand, TResponse>(
    ILogger<LoggingBehavior<TCommand, TResponse>> logger)
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(
        TCommand command,
        Func<Task<Result<TResponse>>> next,
        CancellationToken cancellationToken)
    {
        string commandName = typeof(TCommand).Name;

        logger.LogInformation("Processing command {Command}", commandName);

        Result<TResponse> result = await next();

        if (result.IsSuccess)
        {
            logger.LogInformation("Completed command {Command}", commandName);
        }
        else
        {
            using (LogContext.PushProperty("Error", result.Error, true))
            {
                logger.LogError("Completed command {Command} with error", commandName);
            }
        }

        return result;
    }
}
