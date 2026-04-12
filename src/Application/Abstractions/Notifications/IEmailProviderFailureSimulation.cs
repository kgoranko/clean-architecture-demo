namespace Application.Abstractions.Notifications;

public interface IEmailProviderFailureSimulation
{
    bool ShouldFailWelcomeEmail { get; }

    bool WelcomeEmailFailed { get; }

    IDisposable FailWelcomeEmail();

    void RecordWelcomeEmailFailure();
}
