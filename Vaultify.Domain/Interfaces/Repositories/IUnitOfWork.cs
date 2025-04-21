namespace Vaultify.Domain.Interfaces.Repositories;

/// <summary>
/// Defines a Unit of Work that coordinates operations across multiple repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// User repository
    /// </summary>
    IUserRepository Users { get; }
    
    /// <summary>
    /// Password vault repository
    /// </summary>
    IPasswordVaultRepository PasswordVaults { get; }
    
    /// <summary>
    /// Saves all changes made in this unit of work to the database
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Begins a database transaction
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
} 