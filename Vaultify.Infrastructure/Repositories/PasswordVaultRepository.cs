using Microsoft.EntityFrameworkCore;
using Vaultify.Domain.Entities;
using Vaultify.Domain.Interfaces;
using Vaultify.Infrastructure.Data;

namespace Vaultify.Infrastructure.Repositories;

public class PasswordVaultRepository : BaseRepository<PasswordVault>, IPasswordVaultRepository
{
    public PasswordVaultRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PasswordVault>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.LastAccessed)
            .ToListAsync(cancellationToken);
    }

    public async Task<PasswordVault> GetByUserIdAndNameAsync(Guid userId, string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Name == name, cancellationToken);
    }

    public async Task<bool> IsNameUniqueForUserAsync(Guid userId, string name, CancellationToken cancellationToken = default)
    {
        return !await _dbSet
            .AnyAsync(p => p.UserId == userId && p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<PasswordVault>> SearchByTitleAsync(Guid userId, string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.UserId == userId && p.Name.Contains(searchTerm))
            .OrderByDescending(p => p.LastAccessed)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PasswordVault>> GetRecentlyAccessedAsync(Guid userId, int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.LastAccessed)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
} 