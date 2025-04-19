using Vaultify.Domain.Entities;

namespace Vaultify.Domain.Interfaces.Services.Users;

/// <summary>
/// Service for managing user accounts
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user account
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="password">User's password</param>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <returns>The created user</returns>
    Task<User> CreateUserAsync(string email, string password, string firstName, string lastName);

    /// <summary>
    /// Authenticates a user by email and password
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="password">User's password</param>
    /// <returns>The authenticated user if successful, null otherwise</returns>
    Task<User> AuthenticateAsync(string email, string password);

    /// <summary>
    /// Changes a user's password
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="currentPassword">Current password</param>
    /// <param name="newPassword">New password</param>
    /// <returns>True if successful, false if current password is incorrect</returns>
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);

    /// <summary>
    /// Checks if an email is already in use
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <returns>True if email is available, false if already in use</returns>
    Task<bool> IsEmailAvailableAsync(string email);

    /// <summary>
    /// Gets a user by their ID
    /// </summary>
    /// <param name="userId">ID of the user to get</param>
    /// <returns>The user, or null if not found</returns>
    Task<User> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Updates a user's profile information
    /// </summary>
    /// <param name="userId">ID of the user to update</param>
    /// <param name="firstName">New first name</param>
    /// <param name="lastName">New last name</param>
    /// <returns>The updated user</returns>
    Task<User> UpdateUserProfileAsync(Guid userId, string firstName, string lastName);
}