using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.DependencyInjection;

namespace Infrastructure.Persistence;

internal sealed class SqlUserRepository(DemoDbContext dbContext) : IUserRepository, IScopedService
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        string normalizedEmail = NormalizeEmail(email);

        return dbContext.Users.SingleOrDefaultAsync(
            user => user.Email == normalizedEmail,
            cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        string normalizedEmail = NormalizeEmail(email);

        return dbContext.Users.AnyAsync(
            user => user.Email == normalizedEmail,
            cancellationToken);
    }

    public async Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        if (dbContext.Entry(user).State == EntityState.Detached)
        {
            bool exists = await dbContext.Users
                .AnyAsync(existingUser => existingUser.Id == user.Id, cancellationToken);

            if (exists)
            {
                dbContext.Users.Update(user);
            }
            else
            {
                dbContext.Users.Add(user);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizeEmail(string email) =>
        (email ?? string.Empty).Trim();
}
