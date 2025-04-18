using Microsoft.EntityFrameworkCore;
using Vaultify.Domain.Entities;
using Vaultify.Domain.Interfaces;
using Vaultify.Infrastructure.Data;

namespace Vaultify.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        return !await _dbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.UpdateLastLogin();
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<PasswordVault>> GetUserVaultsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.PasswordVaults
            .Where(v => v.UserId == userId)
            .ToListAsync(cancellationToken);
    }
} 