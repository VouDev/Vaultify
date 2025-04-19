using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vaultify.Domain.Entities;
using Vaultify.Domain.Exceptions;
using Vaultify.Domain.Interfaces.Repositories;
using Vaultify.Infrastructure.Data;

namespace Vaultify.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for password vault operations
/// </summary>
public class PasswordVaultRepository : BaseRepository<PasswordVault>, IPasswordVaultRepository
{
    public PasswordVaultRepository(ApplicationDbContext context, ILogger<PasswordVaultRepository> logger = null) 
        : base(context, logger)
    {
    }

    public async Task<IEnumerable<PasswordVault>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.LastAccessed)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving password vaults for user {UserId}", userId);
            throw new RepositoryException($"Error retrieving password vaults for user {userId}", ex);
        }
    }

    public async Task<PasswordVault> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Vault name cannot be empty", nameof(name));
                
            return await _dbSet
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Name == name, cancellationToken);
        }
        catch (ArgumentException)
        {
            throw; // Rethrow argument exceptions
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving password vault by name '{Name}' for user {UserId}", name, userId);
            throw new RepositoryException($"Error retrieving password vault by name '{name}' for user {userId}", ex);
        }
    }

    public async Task<IEnumerable<PasswordVault>> SearchByWebsiteAsync(Guid userId, string websiteDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(websiteDomain))
                throw new ArgumentException("Website domain cannot be empty", nameof(websiteDomain));
                
            return await _dbSet
                .Where(p => p.UserId == userId && 
                      (p.Website != null && p.Website.Contains(websiteDomain)))
                .OrderByDescending(p => p.LastAccessed)
                .ToListAsync(cancellationToken);
        }
        catch (ArgumentException)
        {
            throw; // Rethrow argument exceptions
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error searching password vaults by website domain '{Domain}' for user {UserId}", websiteDomain, userId);
            throw new RepositoryException($"Error searching password vaults by website domain '{websiteDomain}' for user {userId}", ex);
        }
    }

    public async Task<bool> IsNameUniqueForUserAsync(Guid userId, string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Vault name cannot be empty", nameof(name));
                
            return !await _dbSet
                .AnyAsync(p => p.UserId == userId && p.Name == name, cancellationToken);
        }
        catch (ArgumentException)
        {
            throw; // Rethrow argument exceptions
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error checking if vault name '{Name}' is unique for user {UserId}", name, userId);
            throw new RepositoryException($"Error checking if vault name '{name}' is unique for user {userId}", ex);
        }
    }

    public async Task<IEnumerable<PasswordVault>> GetRecentlyAccessedAsync(Guid userId, int count, CancellationToken cancellationToken = default)
    {
        try
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than zero", nameof(count));
                
            return await _dbSet
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.LastAccessed)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
        catch (ArgumentException)
        {
            throw; // Rethrow argument exceptions
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving {Count} recently accessed vaults for user {UserId}", count, userId);
            throw new RepositoryException($"Error retrieving recently accessed vaults for user {userId}", ex);
        }
    }
} 