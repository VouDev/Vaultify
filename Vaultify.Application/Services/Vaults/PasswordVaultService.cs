using Microsoft.Extensions.Logging;
using Vaultify.Application.Extensions;
using Vaultify.Application.Interfaces.Services.Vaults;
using Vaultify.Domain.Entities;
using Vaultify.Domain.Interfaces.Repositories;

namespace Vaultify.Application.Services.Vaults;

/// <summary>
/// Implementation of password vault management service
/// </summary>
public class PasswordVaultService : IPasswordVaultService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PasswordVaultService> _logger;

    public PasswordVaultService(IUnitOfWork unitOfWork,
                                ILogger<PasswordVaultService> logger = null)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PasswordVault>> GetAllVaultsAsync(Guid userId)
    {
        try
        {
            return await _unitOfWork.PasswordVaults.GetAllByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving all vaults for user {UserId}", userId);
            throw new Exception($"Error retrieving password vaults", ex);
        }
    }

    /// <inheritdoc />
    public async Task<PasswordVault> GetVaultByIdAsync(Guid vaultId, Guid userId)
    {
        try
        {
            var vault = await _unitOfWork.PasswordVaults.GetByIdAsync(vaultId);
            
            // Authorization check
            if (vault == null || vault.UserId != userId)
                return null;
                
            // Update last accessed time
            return await _unitOfWork.ExecuteInTransactionAsync(() =>
            {
                vault.UpdateLastAccessed();
                _unitOfWork.PasswordVaults.Update(vault);
                return vault;
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving vault {VaultId} for user {UserId}", vaultId, userId);
            throw new Exception($"Error retrieving password vault", ex);
        }
    }

    /// <inheritdoc />
    public async Task<PasswordVault> GetVaultByNameAsync(string name, Guid userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Vault name cannot be empty", nameof(name));
                
            return await _unitOfWork.PasswordVaults.GetByNameAsync(userId, name);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving vault by name '{Name}' for user {UserId}", name, userId);
            throw new Exception($"Error retrieving password vault", ex);
        }
    }

    /// <inheritdoc />
    public async Task<PasswordVault> CreateVaultAsync(Guid userId, string name, string username, string password, string website = "", string notes = "")
    {
        try
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Vault name cannot be empty", nameof(name));
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));
                
            // Check if name is already used by this user
            var existingVault = await _unitOfWork.PasswordVaults.GetByNameAsync(userId, name);
            if (existingVault != null)
                throw new InvalidOperationException($"A vault with the name '{name}' already exists");

            // Create vault in transaction
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var vault = PasswordVault.Create(userId, name, username, password, website, notes);
                await _unitOfWork.PasswordVaults.AddAsync(vault);
                return vault;
            });
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error creating vault '{Name}' for user {UserId}", name, userId);
            throw new Exception("Error creating password vault", ex);
        }
    }

    /// <inheritdoc />
    public async Task<PasswordVault> UpdateVaultAsync(Guid vaultId, Guid userId, string name, string username, string password, string website, string notes)
    {
        try
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Vault name cannot be empty", nameof(name));
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));
                
            // Get the vault and check authorization
            var vault = await _unitOfWork.PasswordVaults.GetByIdAsync(vaultId);
            if (vault == null)
                throw new InvalidOperationException($"Vault with ID {vaultId} not found");
            if (vault.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to modify this vault");
                
            // Check if the new name is already used (if changed)
            if (name != vault.Name)
            {
                var existingVault = await _unitOfWork.PasswordVaults.GetByNameAsync(userId, name);
                if (existingVault != null && existingVault.Id != vaultId)
                    throw new InvalidOperationException($"A vault with the name '{name}' already exists");
            }

            // Update vault in transaction
            return await _unitOfWork.ExecuteInTransactionAsync(() =>
            {
                vault.Update(name, username, password, website, notes);
                _unitOfWork.PasswordVaults.Update(vault);
                return vault;
            });
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating vault {VaultId} for user {UserId}", vaultId, userId);
            throw new Exception("Error updating password vault", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteVaultAsync(Guid vaultId, Guid userId)
    {
        try
        {
            // Get the vault and check authorization
            var vault = await _unitOfWork.PasswordVaults.GetByIdAsync(vaultId);
            if (vault == null)
                return false;
            if (vault.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this vault");

            // Delete vault in transaction
            await _unitOfWork.ExecuteInTransactionAsync(() =>
            {
                _unitOfWork.PasswordVaults.Remove(vault);
            });
            
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting vault {VaultId} for user {UserId}", vaultId, userId);
            throw new Exception("Error deleting password vault", ex);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PasswordVault>> SearchByWebsiteAsync(Guid userId, string websiteDomain)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(websiteDomain))
                throw new ArgumentException("Website domain cannot be empty", nameof(websiteDomain));
                
            return await _unitOfWork.PasswordVaults.SearchByWebsiteAsync(userId, websiteDomain);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error searching vaults by website domain '{Domain}' for user {UserId}", websiteDomain, userId);
            throw new Exception("Error searching password vaults", ex);
        }
    }
} 