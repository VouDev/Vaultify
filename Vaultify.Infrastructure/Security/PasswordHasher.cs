using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Vaultify.Domain.Interfaces.Security;

namespace Vaultify.Infrastructure.Security;

/// <summary>
/// Implements password hashing functionality using PBKDF2 with HMAC-SHA256
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int IterationCount = 10000;
    private const int NumBytesRequested = 256 / 8; // 256 bits
    private const int SaltSize = 128 / 8; // 128 bits
    
    /// <inheritdoc />
    public (string Hash, string Salt) HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));

        // Generate a random salt
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        
        // Generate the hash
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: IterationCount,
            numBytesRequested: NumBytesRequested);

        // Convert to base64 for storage
        string hashString = Convert.ToBase64String(hash);
        string saltString = Convert.ToBase64String(salt);
        
        return (hashString, saltString);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string password, string hash, string salt)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;
        
        if (string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(salt))
            return false;

        try
        {
            // Convert salt from base64
            byte[] saltBytes = Convert.FromBase64String(salt);
            
            // Generate hash from password and salt
            byte[] hashBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: NumBytesRequested);
            
            // Compare the generated hash with the stored hash
            string computedHash = Convert.ToBase64String(hashBytes);
            return hash == computedHash;
        }
        catch
        {
            // If there's any exception (invalid base64, etc), return false
            return false;
        }
    }
} 