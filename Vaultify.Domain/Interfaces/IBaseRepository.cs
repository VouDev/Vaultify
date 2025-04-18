using System.Linq.Expressions;

namespace Vaultify.Domain.Interfaces;

// This interface defines the contract for all repositories in our application.
// It provides common database operations that can be used by any entity.
// The generic type T represents the entity type (e.g., User, PasswordVault).
public interface IBaseRepository<T> where T : class
{
    // Basic CRUD operations
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Unit of Work pattern methods
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    
    // Save changes explicitly (for Unit of Work pattern)
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 