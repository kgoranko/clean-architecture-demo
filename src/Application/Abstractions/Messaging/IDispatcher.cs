using SharedKernel;

namespace Application.Abstractions.Messaging;

public interface IDispatcher
{
    Task<Result<TResponse>> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct = default)
        where TRequest : IRequest<TResponse>;
}
