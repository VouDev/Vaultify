using Vaultify.Domain.Entities;

namespace Vaultify.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for password vault operations
/// </summary>
public interface IPasswordVaultRepository : IBaseRepository<PasswordVault>
{
    /// <summary>
    /// Gets all password vaults for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of password vaults</returns>
    Task<IEnumerable<PasswordVault>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a user's password vault by name
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="name">The name of the vault</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The password vault if found, null otherwise</returns>
    Task<PasswordVault> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Searches for password vaults by website domain
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="websiteDomain">The website domain to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of matching password vaults</returns>
    Task<IEnumerable<PasswordVault>> SearchByWebsiteAsync(Guid userId, string websiteDomain, CancellationToken cancellationToken = default);
} 