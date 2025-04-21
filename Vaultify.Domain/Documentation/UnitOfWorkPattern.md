# Unit of Work Pattern in Vaultify

## Overview

The Unit of Work pattern is an architectural pattern used to maintain a list of business transactions that affect the database and coordinate the writing of changes to ensure data consistency. In our application, this pattern provides a clean abstraction around database operations and transaction management.

## Key Components

### IUnitOfWork Interface

The `IUnitOfWork` interface defines the contract for our Unit of Work implementation:

```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IPasswordVaultRepository PasswordVaults { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

### UnitOfWork Implementation

The implementation manages access to repositories and transaction handling:

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction _transaction;
    
    public IUserRepository Users { get; }
    public IPasswordVaultRepository PasswordVaults { get; }
    
    // Constructor, methods, and IDisposable implementation...
}
```

### Extension Methods

Extension methods make transaction handling more elegant:

```csharp
public static async Task<T> ExecuteInTransactionAsync<T>(
    this IUnitOfWork unitOfWork,
    Func<Task<T>> operation)
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
```

## Benefits of Unit of Work Pattern

1. **Maintains Data Integrity**: 
   - Ensures that either all operations complete successfully or none of them do
   - Prevents partial updates to the database

2. **Simplifies Transaction Management**:
   - Centralizes transaction handling logic
   - Provides a consistent approach to managing database transactions

3. **Improves Code Maintainability**:
   - Separates data access from business logic
   - Reduces code duplication for transaction management

4. **Enhances Testability**:
   - Facilitates mocking of database operations
   - Makes services easier to test in isolation

5. **Single Point of Change**:
   - Changes to transaction handling need to be made in one place only
   - Makes the codebase more maintainable

## Usage Examples

### Basic Usage

```csharp
// Get data without transaction
var user = await _unitOfWork.Users.GetByIdAsync(userId);
```

### Using Transactions

```csharp
// Update within a transaction
return await _unitOfWork.ExecuteInTransactionAsync(() =>
{
    user.UpdateName(firstName, lastName);
    _unitOfWork.Users.Update(user);
    return Task.FromResult(user);
});
```

### Multiple Operations in One Transaction

```csharp
await _unitOfWork.ExecuteInTransactionAsync(async () =>
{
    // Create user
    var user = User.Create(email, hash, salt, firstName, lastName);
    await _unitOfWork.Users.AddAsync(user);
    
    // Create initial vault
    var vault = PasswordVault.Create(user.Id, "Default", "username", "password");
    await _unitOfWork.PasswordVaults.AddAsync(vault);
    
    return user;
});
```

## Implementation Considerations

1. **Repository Lifetime**:
   - Repositories should be scoped to the same lifetime as the Unit of Work
   - All repositories should share the same DbContext instance

2. **Transaction Boundaries**:
   - Clearly define where transactions begin and end
   - Keep transactions as short as possible

3. **Exception Handling**:
   - Always roll back transactions when exceptions occur
   - Ensure proper disposal of resources

4. **Concurrency**:
   - Be aware of concurrency issues when multiple operations modify the same data
   - Consider using optimistic concurrency with EF Core

## Design Decisions

1. **Repository Access Through Unit of Work**:
   - Repositories are accessed as properties of the Unit of Work
   - This ensures all operations use the same DbContext

2. **No SaveChanges in Repositories**:
   - Repositories do not call SaveChanges
   - Unit of Work is responsible for persisting changes

3. **Extension Methods for Cleaner Code**:
   - Extension methods simplify transaction handling
   - Makes service code more readable and maintainable 