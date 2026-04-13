using Application.Abstractions.Messaging;
using Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;
using SharedKernel.DependencyInjection;

namespace Application.Common;

public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher, IScopedService
{
    public Task<Result<TResponse>> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct = default)
        where TRequest : IRequest<TResponse>
    {
        RequestExecutor<TRequest, TResponse> executor =
            serviceProvider.GetRequiredService<RequestExecutor<TRequest, TResponse>>();

        return executor.Execute(request, ct);
    }
}
