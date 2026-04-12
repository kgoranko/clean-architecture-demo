using Application.Abstractions.Notifications;
using SharedKernel.DependencyInjection;

namespace Infrastructure.Providers;

internal sealed class EmailProviderFailureSimulation : IEmailProviderFailureSimulation, ISingletonService
{
    private static readonly AsyncLocal<FailureState?> Current = new();

    public bool ShouldFailWelcomeEmail => Current.Value?.ShouldFailWelcomeEmail == true;

    public bool WelcomeEmailFailed => Current.Value?.WelcomeEmailFailed == true;

    public IDisposable FailWelcomeEmail()
    {
        FailureState? previousState = Current.Value;
        Current.Value = new FailureState(shouldFailWelcomeEmail: true);

        return new FailureScope(previousState);
    }

    public void RecordWelcomeEmailFailure()
    {
        if (Current.Value is { } state)
        {
            state.WelcomeEmailFailed = true;
        }
    }

    private sealed class FailureState(bool shouldFailWelcomeEmail)
    {
        public bool ShouldFailWelcomeEmail { get; } = shouldFailWelcomeEmail;

        public bool WelcomeEmailFailed { get; set; }
    }

    private sealed class FailureScope(FailureState? previousState) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Current.Value = previousState;
            _disposed = true;
        }
    }
}
