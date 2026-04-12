using SharedKernel;
using SharedKernel.DependencyInjection;

namespace Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider, ISingletonService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
