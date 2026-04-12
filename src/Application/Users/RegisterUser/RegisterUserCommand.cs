using Application.Abstractions.Messaging;

namespace Application.Users.RegisterUser;

/// <summary>
/// A CQRS command sent through ICommandDispatcher.
/// The dispatcher runs behaviors and then invokes the matching handler.
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string EmailTrigger) : ICommand<Guid>;
