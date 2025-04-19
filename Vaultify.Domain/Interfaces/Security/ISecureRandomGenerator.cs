namespace Vaultify.Domain.Interfaces.Security;

/// <summary>
/// Provides functionality for generating cryptographically secure random data
/// </summary>
public interface ISecureRandomGenerator
{
    /// <summary>
    /// Generates a random string of the specified length
    /// </summary>
    /// <param name="length">The length of the string to generate</param>
    /// <param name="includeSpecialChars">Whether to include special characters</param>
    /// <returns>A random string</returns>
    string GenerateRandomString(int length, bool includeSpecialChars = true);
    
    /// <summary>
    /// Generates a random byte array of the specified length
    /// </summary>
    /// <param name="length">The length of the byte array to generate</param>
    /// <returns>A random byte array</returns>
    byte[] GenerateRandomBytes(int length);
    
    /// <summary>
    /// Generates a secure password based on specified requirements
    /// </summary>
    /// <param name="length">The length of the password</param>
    /// <param name="includeSpecialChars">Whether to include special characters</param>
    /// <param name="includeNumbers">Whether to include numbers</param>
    /// <param name="includeUppercase">Whether to include uppercase letters</param>
    /// <returns>A secure random password</returns>
    string GenerateSecurePassword(int length, bool includeSpecialChars = true, 
        bool includeNumbers = true, bool includeUppercase = true);
} 