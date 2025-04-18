using Vaultify.Domain.Entities;

namespace Vaultify.Domain.Interfaces;

public interface IPasswordVaultRepository : IBaseRepository<PasswordVault>
{
    Task<IEnumerable<PasswordVault>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PasswordVault> GetByUserIdAndNameAsync(Guid userId, string name, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueForUserAsync(Guid userId, string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<PasswordVault>> SearchByTitleAsync(Guid userId, string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<PasswordVault>> GetRecentlyAccessedAsync(Guid userId, int count, CancellationToken cancellationToken = default);
} 