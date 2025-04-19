namespace Vaultify.Domain.Interfaces.Repositories;

/// <summary>
/// Defines a generic repository for basic CRUD operations on entities
/// </summary>
/// <typeparam name="T">The entity type this repository manages</typeparam>
public interface IBaseRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its unique identifier
    /// </summary>
    /// <param name="id">The entity's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of all entities</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a new entity
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    void Update(T entity);
    
    /// <summary>
    /// Removes an entity
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    void Remove(T entity);
} 