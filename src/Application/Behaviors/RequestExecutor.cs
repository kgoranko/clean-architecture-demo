using Application.Abstractions.Behaviors;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Behaviors;

public sealed class RequestExecutor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _handler;
    private readonly IEnumerable<IRequestBehavior<TRequest, TResponse>> _behaviors;

    public RequestExecutor(
        IRequestHandler<TRequest, TResponse> handler,
        IEnumerable<IRequestBehavior<TRequest, TResponse>> behaviors)
    {
        _handler = handler;
        _behaviors = behaviors;
    }

    public Task<Result<TResponse>> Execute(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        Func<Task<Result<TResponse>>> next =
            () => _handler.Handle(request, cancellationToken);

        foreach (IRequestBehavior<TRequest, TResponse> behavior in _behaviors.Reverse())
        {
            Func<Task<Result<TResponse>>> current = next;
            next = () => behavior.Handle(request, current, cancellationToken);
        }

        return next();
    }
}
