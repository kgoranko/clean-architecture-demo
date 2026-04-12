using Application.Abstractions.Behaviors;
using Application.Abstractions.Messaging;
using FluentValidation;
using FluentValidation.Results;
using SharedKernel;

namespace Application.Behaviors;

internal sealed class ValidationBehavior<TCommand, TResponse>(
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(
        TCommand command,
        Func<Task<Result<TResponse>>> next,
        CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures = await ValidateAsync(command, cancellationToken);

        if (validationFailures.Length == 0)
        {
            return await next();
        }

        return Result.Failure<TResponse>(CreateValidationError(validationFailures));
    }

    private async Task<ValidationFailure[]> ValidateAsync(
        TCommand command,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        return validationResults
            .Where(static validationResult => !validationResult.IsValid)
            .SelectMany(static validationResult => validationResult.Errors)
            .ToArray();
    }

    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());
}
