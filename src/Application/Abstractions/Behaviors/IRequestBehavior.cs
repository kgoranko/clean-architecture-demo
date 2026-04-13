using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Abstractions.Behaviors;

public interface IRequestBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> Handle(
        TRequest request,
        Func<Task<Result<TResponse>>> next,
        CancellationToken cancellationToken);
}
