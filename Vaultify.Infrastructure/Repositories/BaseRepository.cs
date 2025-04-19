using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vaultify.Domain.Exceptions;
using Vaultify.Domain.Interfaces.Repositories;
using Vaultify.Infrastructure.Data;

namespace Vaultify.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for basic CRUD operations
/// </summary>
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<BaseRepository<T>> _logger;

    protected BaseRepository(ApplicationDbContext context, ILogger<BaseRepository<T>> logger = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving {EntityType} with ID {Id}", typeof(T).Name, id);
            throw new RepositoryException($"Error retrieving {typeof(T).Name}", ex);
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving all {EntityType} entities", typeof(T).Name);
            throw new RepositoryException($"Error retrieving all {typeof(T).Name} entities", ex);
        }
    }

    public virtual async Task<T> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error finding {EntityType} entity with predicate", typeof(T).Name);
            throw new RepositoryException($"Error finding {typeof(T).Name} entity", ex);
        }
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error finding {EntityType} entities with predicate", typeof(T).Name);
            throw new RepositoryException($"Error finding {typeof(T).Name} entities", ex);
        }
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error checking existence of {EntityType} with predicate", typeof(T).Name);
            throw new RepositoryException($"Error checking existence of {typeof(T).Name}", ex);
        }
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error counting {EntityType} entities with predicate", typeof(T).Name);
            throw new RepositoryException($"Error counting {typeof(T).Name} entities", ex);
        }
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            await _dbSet.AddAsync(entity, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger?.LogWarning(ex, "Attempted to add null {EntityType} entity", typeof(T).Name);
            throw; 
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding {EntityType} entity", typeof(T).Name);
            throw new RepositoryException($"Error adding {typeof(T).Name} entity", ex);
        }
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
                
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        catch (ArgumentNullException ex)
        {
            _logger?.LogWarning(ex, "Attempted to add null collection of {EntityType} entities", typeof(T).Name);
            throw; 
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding range of {EntityType} entities", typeof(T).Name);
            throw new RepositoryException($"Error adding range of {typeof(T).Name} entities", ex);
        }
    }

    public virtual void Update(T entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            _dbSet.Update(entity);
        }
        catch (ArgumentNullException ex)
        {
            _logger?.LogWarning(ex, "Attempted to update null {EntityType} entity", typeof(T).Name);
            throw; 
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating {EntityType} entity", typeof(T).Name);
            throw new RepositoryException($"Error updating {typeof(T).Name} entity", ex);
        }
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        try
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
                
            _dbSet.UpdateRange(entities);
        }
        catch (ArgumentNullException ex)
        {
            _logger?.LogWarning(ex, "Attempted to update null collection of {EntityType} entities", typeof(T).Name);
            throw; 
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating range of {EntityType} entities", typeof(T).Name);
            throw new RepositoryException($"Error updating range of {typeof(T).Name} entities", ex);
        }
    }

    public virtual void Remove(T entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            _dbSet.Remove(entity);
        }
        catch (ArgumentNullException ex)
        {
            _logger?.LogWarning(ex, "Attempted to remove null {EntityType} entity", typeof(T).Name);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error removing {EntityType} entity", typeof(T).Name);
            throw new RepositoryException($"Error removing {typeof(T).Name} entity", ex);
        }
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        try
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
                
            _dbSet.RemoveRange(entities);
        }
        catch (ArgumentNullException ex)
        {
            _logger?.LogWarning(ex, "Attempted to remove null collection of {EntityType} entities", typeof(T).Name);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error removing range of {EntityType} entities", typeof(T).Name);
            throw new RepositoryException($"Error removing range of {typeof(T).Name} entities", ex);
        }
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger?.LogError(ex, "Concurrency conflict when saving changes for {EntityType}", typeof(T).Name);
            throw new RepositoryException("A concurrency conflict occurred while saving changes. The entity may have been modified or deleted by another process.", ex);
        }
        catch (DbUpdateException ex)
        {
            _logger?.LogError(ex, "Database update error when saving changes for {EntityType}", typeof(T).Name);
            throw new RepositoryException("An error occurred while saving changes to the database. This may be due to a constraint violation or other database error.", ex);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error when saving changes for {EntityType}", typeof(T).Name);
            throw new RepositoryException("An unexpected error occurred while saving changes to the database.", ex);
        }
    }
} 