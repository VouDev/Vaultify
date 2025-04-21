using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Vaultify.Domain.Interfaces.Repositories;
using Vaultify.Infrastructure.Data;

namespace Vaultify.Infrastructure.Repositories;

/// <summary>
/// Implementation of the Unit of Work pattern
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction _transaction;
    private bool _disposed;

    public IUserRepository Users { get; }
    public IPasswordVaultRepository PasswordVaults { get; }

    public UnitOfWork(ApplicationDbContext context,
                      IUserRepository userRepository,
                      IPasswordVaultRepository passwordVaultRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        PasswordVaults = passwordVaultRepository ?? throw new ArgumentNullException(nameof(passwordVaultRepository));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
            }

            _disposed = true;
        }
    }
} 