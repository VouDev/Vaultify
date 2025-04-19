using System.Security.Cryptography;
using System.Text;
using Vaultify.Domain.Interfaces.Security;

namespace Vaultify.Infrastructure.Security;

/// <summary>
/// Implements encryption functionality using AES-256 encryption
/// </summary>
public class EncryptionService : IEncryptionService
{
    private const int KeySize = 256;
    private const int BlockSize = 128;
    private const int IvSize = 16; // 128 bits
    private readonly ISecureRandomGenerator _randomGenerator;

    public EncryptionService(ISecureRandomGenerator randomGenerator)
    {
        _randomGenerator = randomGenerator;
    }

    /// <inheritdoc />
    public string Encrypt(string plainText, string masterKey)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        if (string.IsNullOrEmpty(masterKey))
            throw new ArgumentException("Master key cannot be null or empty", nameof(masterKey));

        try
        {
            // Generate a random IV for each encryption operation
            byte[] iv = _randomGenerator.GenerateRandomBytes(IvSize);
            
            // Derive encryption key from master key
            using var keyDerivation = new Rfc2898DeriveBytes(
                masterKey, 
                iv, 
                10000, 
                HashAlgorithmName.SHA256);
            byte[] key = keyDerivation.GetBytes(KeySize / 8);

            // Create encryptor
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Encrypt the data
            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Combine IV and ciphertext
            byte[] result = new byte[IvSize + cipherBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, IvSize);
            Buffer.BlockCopy(cipherBytes, 0, result, IvSize, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Encryption failed", ex);
        }
    }

    /// <inheritdoc />
    public string Decrypt(string encryptedData, string masterKey)
    {
        if (string.IsNullOrEmpty(encryptedData))
            return string.Empty;

        if (string.IsNullOrEmpty(masterKey))
            throw new ArgumentException("Master key cannot be null or empty", nameof(masterKey));

        try
        {
            // Get complete data as bytes
            byte[] cipherWithIv = Convert.FromBase64String(encryptedData);

            // Extract IV from the beginning
            byte[] iv = new byte[IvSize];
            Buffer.BlockCopy(cipherWithIv, 0, iv, 0, IvSize);

            // Extract the ciphertext
            byte[] cipherText = new byte[cipherWithIv.Length - IvSize];
            Buffer.BlockCopy(cipherWithIv, IvSize, cipherText, 0, cipherText.Length);

            // Derive decryption key from master key
            using var keyDerivation = new Rfc2898DeriveBytes(
                masterKey, 
                iv, 
                10000, 
                HashAlgorithmName.SHA256);
            byte[] key = keyDerivation.GetBytes(KeySize / 8);

            // Create decryptor
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Decrypt
            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Decryption failed", ex);
        }
    }

    /// <inheritdoc />
    public string GenerateKey()
    {
        // Generate a secure random key
        byte[] keyBytes = _randomGenerator.GenerateRandomBytes(KeySize / 8);
        return Convert.ToBase64String(keyBytes);
    }
} 