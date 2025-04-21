using Vaultify.Domain.Interfaces.Repositories;

namespace Vaultify.Application.Extensions;

/// <summary>
/// Extension methods for IUnitOfWork
/// </summary>
public static class UnitOfWorkExtensions
{
    /// <summary>
    /// Executes an asynchronous operation within a transaction
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="operation">The asynchronous operation to execute</param>
    /// <returns>The result of the operation</returns>
    public static async Task<T> ExecuteInTransactionAsync<T>(this IUnitOfWork unitOfWork, Func<Task<T>> operation)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            var result = await operation();
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();
            return result;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Executes a synchronous operation within a transaction
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="operation">The synchronous operation to execute</param>
    /// <returns>The result of the operation</returns>
    public static async Task<T> ExecuteInTransactionAsync<T>(this IUnitOfWork unitOfWork, Func<T> operation)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            var result = operation();
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();
            return result;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Executes an asynchronous operation within a transaction
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="operation">The asynchronous operation to execute</param>
    public static async Task ExecuteInTransactionAsync(this IUnitOfWork unitOfWork, Func<Task> operation)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            await operation();
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Executes a synchronous operation within a transaction
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="operation">The synchronous operation to execute</param>
    public static async Task ExecuteInTransactionAsync(this IUnitOfWork unitOfWork, Action operation)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            operation();
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
} 