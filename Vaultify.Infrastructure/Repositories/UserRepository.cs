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
/// Repository implementation for User entity operations
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger = null) 
        : base(context, logger)
    {
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
                
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
        catch (ArgumentException)
        {
            throw; // Rethrow argument exceptions
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving user with email {Email}", email);
            throw new RepositoryException($"Error retrieving user with email {email}", ex);
        }
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
                
            return !await _dbSet
                .AnyAsync(u => u.Email == email, cancellationToken);
        }
        catch (ArgumentException)
        {
            throw; // Rethrow argument exceptions
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error checking if email {Email} is unique", email);
            throw new RepositoryException($"Error checking if email {email} is unique", ex);
        }
    }

    public async Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.UpdateLastLogin();
                await SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new RepositoryException($"User with ID {userId} not found");
            }
        }
        catch (RepositoryException)
        {
            throw; // Rethrow already wrapped repository exceptions
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating last login for user {UserId}", userId);
            throw new RepositoryException($"Error updating last login for user {userId}", ex);
        }
    }

    public async Task<IEnumerable<PasswordVault>> GetUserVaultsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.PasswordVaults
                .Where(v => v.UserId == userId)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving vaults for user {UserId}", userId);
            throw new RepositoryException($"Error retrieving vaults for user {userId}", ex);
        }
    }
} 