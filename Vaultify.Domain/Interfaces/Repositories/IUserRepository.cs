using Vaultify.Domain.Entities;

namespace Vaultify.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    /// Gets a user by their email address
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if an email is unique (not already used by any user)
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the email is unique, false otherwise</returns>
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all password vaults belonging to a specific user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of the user's password vaults</returns>
    Task<IEnumerable<PasswordVault>> GetUserVaultsAsync(Guid userId, CancellationToken cancellationToken = default);
} 