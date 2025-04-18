using Vaultify.Domain.Entities;

namespace Vaultify.Domain.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    // User-specific operations
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
    Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PasswordVault>> GetUserVaultsAsync(Guid userId, CancellationToken cancellationToken = default);
} 