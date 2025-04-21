using Vaultify.Domain.Entities;

namespace Vaultify.Domain.Interfaces.Services.Vaults;

/// <summary>
/// Service for managing password vaults
/// </summary>
public interface IPasswordVaultService
{
    /// <summary>
    /// Gets all password vaults for a user
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <returns>Collection of password vaults</returns>
    Task<IEnumerable<PasswordVault>> GetAllVaultsAsync(Guid userId);
    
    /// <summary>
    /// Gets a password vault by its ID
    /// </summary>
    /// <param name="vaultId">ID of the vault</param>
    /// <param name="userId">ID of the user (for authorization)</param>
    /// <returns>The password vault if found and authorized, null otherwise</returns>
    Task<PasswordVault> GetVaultByIdAsync(Guid vaultId, Guid userId);
    
    /// <summary>
    /// Gets a password vault by its name
    /// </summary>
    /// <param name="name">Name of the vault</param>
    /// <param name="userId">ID of the user</param>
    /// <returns>The password vault if found, null otherwise</returns>
    Task<PasswordVault> GetVaultByNameAsync(string name, Guid userId);
    
    /// <summary>
    /// Creates a new password vault
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="name">Name of the vault</param>
    /// <param name="username">Username stored in the vault</param>
    /// <param name="password">Password stored in the vault</param>
    /// <param name="website">Website associated with the vault (optional)</param>
    /// <param name="notes">Additional notes (optional)</param>
    /// <returns>The created password vault</returns>
    Task<PasswordVault> CreateVaultAsync(Guid userId, string name, string username, string password, string website = "", string notes = "");
    
    /// <summary>
    /// Updates an existing password vault
    /// </summary>
    /// <param name="vaultId">ID of the vault to update</param>
    /// <param name="userId">ID of the user (for authorization)</param>
    /// <param name="name">Updated name</param>
    /// <param name="username">Updated username</param>
    /// <param name="password">Updated password</param>
    /// <param name="website">Updated website</param>
    /// <param name="notes">Updated notes</param>
    /// <returns>The updated password vault</returns>
    Task<PasswordVault> UpdateVaultAsync(Guid vaultId, Guid userId, string name, string username, string password, string website, string notes);
    
    /// <summary>
    /// Deletes a password vault
    /// </summary>
    /// <param name="vaultId">ID of the vault to delete</param>
    /// <param name="userId">ID of the user (for authorization)</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteVaultAsync(Guid vaultId, Guid userId);
    
    /// <summary>
    /// Searches for password vaults by website domain
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="websiteDomain">Website domain to search for</param>
    /// <returns>Collection of matching password vaults</returns>
    Task<IEnumerable<PasswordVault>> SearchByWebsiteAsync(Guid userId, string websiteDomain);
} 