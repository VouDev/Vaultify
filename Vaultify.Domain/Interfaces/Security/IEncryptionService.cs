namespace Vaultify.Domain.Interfaces.Security;

/// <summary>
/// Provides functionality for encrypting and decrypting sensitive data
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts plain text data using a secure algorithm
    /// </summary>
    /// <param name="plainText">The text to encrypt</param>
    /// <param name="masterKey">The master key used for encryption</param>
    /// <returns>The encrypted data</returns>
    string Encrypt(string plainText, string masterKey);
    
    /// <summary>
    /// Decrypts encrypted data back to plain text
    /// </summary>
    /// <param name="encryptedData">The encrypted data</param>
    /// <param name="masterKey">The master key used for encryption</param>
    /// <returns>The decrypted plain text</returns>
    string Decrypt(string encryptedData, string masterKey);
    
    /// <summary>
    /// Generates a new random encryption key
    /// </summary>
    /// <returns>A secure random encryption key</returns>
    string GenerateKey();
} 