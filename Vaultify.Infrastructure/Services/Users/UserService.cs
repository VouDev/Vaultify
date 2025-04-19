using Microsoft.Extensions.Logging;
using Vaultify.Domain.Entities;
using Vaultify.Domain.Interfaces.Repositories;
using Vaultify.Domain.Interfaces.Security;
using Vaultify.Domain.Interfaces.Services.Users;
using Vaultify.Infrastructure.Data;

namespace Vaultify.Infrastructure.Services.Users;

/// <summary>
/// Implementation of user account management service
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ApplicationDbContext dbContext,
        ILogger<UserService> logger = null)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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

            // Hash the password
            (string hash, string salt) = _passwordHasher.HashPassword(password);

            // Create the user
            var user = User.Create(email, hash, salt, firstName, lastName);

            // Save to database
            await _userRepository.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }
        catch (ArgumentException ex)
        {
            _logger?.LogWarning(ex, "Invalid argument when creating user: {Message}", ex.Message);
            throw; // Rethrow as these are validation errors that should be handled by the caller
        }
        catch (InvalidOperationException ex)
        {
            _logger?.LogWarning(ex, "Business rule violation when creating user: {Message}", ex.Message);
            throw; // Rethrow as these are business rule violations that should be handled by the caller
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
            var user = await _userRepository.GetByEmailAsync(email, CancellationToken.None);
            if (user == null)
                return null;

            // Verify password
            var isPasswordValid = _passwordHasher.VerifyPassword(password, user.PasswordHash, user.Salt);
            if (!isPasswordValid)
                return null;

            // Update last login time
            user.UpdateLastLogin();
            await _dbContext.SaveChangesAsync();

            return user;
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
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Verify current password
            var isCurrentPasswordValid = _passwordHasher.VerifyPassword(currentPassword, user.PasswordHash, user.Salt);
            if (!isCurrentPasswordValid)
                return false;

            // Hash new password
            (string hash, string salt) = _passwordHasher.HashPassword(newPassword);

            // Update user with reflection to bypass encapsulation for this special case
            var userType = typeof(User);
            var passwordHashProperty = userType.GetProperty("PasswordHash");
            var saltProperty = userType.GetProperty("Salt");

            if (passwordHashProperty != null && saltProperty != null)
            {
                passwordHashProperty.SetValue(user, hash);
                saltProperty.SetValue(user, salt);
            }
            else
            {
                throw new InvalidOperationException("Unable to update password properties");
            }

            _userRepository.Update(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger?.LogError(ex, "Error accessing password properties for user {UserId}", userId);
            throw new Exception("Unable to change password due to a system error", ex);
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

            return await _userRepository.IsEmailUniqueAsync(email, CancellationToken.None);
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
            return await _userRepository.GetByIdAsync(userId);
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

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found");

            user.UpdateName(firstName, lastName);
            _userRepository.Update(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }
        catch (ArgumentException ex)
        {
            _logger?.LogWarning(ex, "Invalid argument when updating user profile: {Message}", ex.Message);
            throw; // Rethrow as these are validation errors that should be handled by the caller
        }
        catch (InvalidOperationException ex)
        {
            _logger?.LogWarning(ex, "User not found during profile update: {Message}", ex.Message);
            throw; // Rethrow as these indicate resource not found
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error updating profile for user {UserId}", userId);
            throw new Exception("Failed to update user profile due to a system error", ex);
        }
    }
} 