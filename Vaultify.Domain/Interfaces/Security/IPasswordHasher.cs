namespace Vaultify.Domain.Interfaces.Security;

/// <summary>
/// Provides functionality for secure password hashing and verification
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password using a secure algorithm
    /// </summary>
    /// <param name="password">The plain text password to hash</param>
    /// <returns>A tuple containing the password hash and salt</returns>
    (string Hash, string Salt) HashPassword(string password);
    
    /// <summary>
    /// Verifies a password against a stored hash
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <param name="hash">The stored password hash</param>
    /// <param name="salt">The salt used when hashing the password</param>
    /// <returns>True if the password matches the hash, false otherwise</returns>
    bool VerifyPassword(string password, string hash, string salt);
} 