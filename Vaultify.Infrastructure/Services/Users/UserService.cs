using Microsoft.Extensions.Logging;
using Vaultify.Domain.Entities;
using Vaultify.Domain.Interfaces.Repositories;
using Vaultify.Domain.Interfaces.Security;
using Vaultify.Domain.Interfaces.Services.Users;
using Vaultify.Infrastructure.Extensions;

namespace Vaultify.Infrastructure.Services.Users;

/// <summary>
/// Implementation of user account management service
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<UserService> logger = null)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<User> CreateUserAsync(string email, string password, string firstName, string lastName)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));

            // Check if email is already in use
            bool isEmailAvailable = await IsEmailAvailableAsync(email);
            if (!isEmailAvailable)
                throw new InvalidOperationException($"Email {email} is already in use");

            // Use Unit of Work with transaction
            return await _unitOfWork.ExecuteInTransactionAsync(async () => 
            {
                // Hash the password
                (string hash, string salt) = _passwordHasher.HashPassword(password);

                // Create the user
                var user = User.Create(email, hash, salt, firstName, lastName);

                // Save to database (transaction will handle SaveChanges)
                await _unitOfWork.Users.AddAsync(user);
                
                return user;
            });
        }
        catch (ArgumentException ex)
        {
            _logger?.LogWarning(ex, "Invalid argument when creating user: {Message}", ex.Message);
            throw; 
        }
        catch (InvalidOperationException ex)
        {
            _logger?.LogWarning(ex, "Business rule violation when creating user: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error occurred while creating user with email {Email}", email);
            throw new Exception("An unexpected error occurred while creating the user account", ex);
        }
    }

    /// <inheritdoc />
    public async Task<User> AuthenticateAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            // Find user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(email, CancellationToken.None);
            if (user == null)
                return null;

            // Verify password
            var isPasswordValid = _passwordHasher.VerifyPassword(password, user.PasswordHash, user.Salt);
            if (!isPasswordValid)
                return null;

            // Update last login time using transaction
            return await _unitOfWork.ExecuteInTransactionAsync(() =>
            {
                user.UpdateLastLogin();
                return user;
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred during authentication for email {Email}", email);
            throw new Exception("Authentication failed due to a system error", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                return false;

            // Get user
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Verify current password
            var isCurrentPasswordValid = _passwordHasher.VerifyPassword(currentPassword, user.PasswordHash, user.Salt);
            if (!isCurrentPasswordValid)
                return false;

            // Change password in transaction
            await _unitOfWork.ExecuteInTransactionAsync(() =>
            {
                // Hash new password
                (string hash, string salt) = _passwordHasher.HashPassword(newPassword);

                // Update user password using domain method
                user.UpdatePassword(hash, salt);
                
                _unitOfWork.Users.Update(user);
            });

            return true;
        }
        catch (ArgumentException ex)
        {
            _logger?.LogError(ex, "Invalid argument when changing password for user {UserId}", userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error changing password for user {UserId}", userId);
            throw new Exception("Failed to change password due to a system error", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _unitOfWork.Users.IsEmailUniqueAsync(email, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error checking email availability for {Email}", email);
            throw new Exception("Unable to verify email availability", ex);
        }
    }

    /// <inheritdoc />
    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        try
        {
            return await _unitOfWork.Users.GetByIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving user with ID {UserId}", userId);
            throw new Exception($"Unable to retrieve user information", ex);
        }
    }

    /// <inheritdoc />
    public async Task<User> UpdateUserProfileAsync(Guid userId, string firstName, string lastName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Name fields cannot be empty");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found");

            // Update profile in transaction
            return await _unitOfWork.ExecuteInTransactionAsync(() =>
            {
                user.UpdateName(firstName, lastName);
                _unitOfWork.Users.Update(user);
                return user;
            });
        }
        catch (ArgumentException ex)
        {
            _logger?.LogWarning(ex, "Invalid argument when updating user profile: {Message}", ex.Message);
            throw; 
        }
        catch (InvalidOperationException ex)
        {
            _logger?.LogWarning(ex, "User not found during profile update: {Message}", ex.Message);
            throw; 
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error updating profile for user {UserId}", userId);
            throw new Exception("Failed to update user profile due to a system error", ex);
        }
    }
} 