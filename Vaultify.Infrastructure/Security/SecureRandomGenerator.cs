using System.Security.Cryptography;
using System.Text;
using Vaultify.Domain.Interfaces.Security;

namespace Vaultify.Infrastructure.Security;

/// <summary>
/// Implements secure random data generation functionality
/// </summary>
public class SecureRandomGenerator : ISecureRandomGenerator
{
    private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
    private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string NumericChars = "0123456789";
    private const string SpecialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

    /// <inheritdoc />
    public byte[] GenerateRandomBytes(int length)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than zero", nameof(length));

        byte[] bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }

    /// <inheritdoc />
    public string GenerateRandomString(int length, bool includeSpecialChars = true)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than zero", nameof(length));

        // Determine character set
        string chars = LowercaseChars + UppercaseChars + NumericChars;
        if (includeSpecialChars)
            chars += SpecialChars;

        var sb = new StringBuilder(length);
        byte[] randomBytes = GenerateRandomBytes(length);

        // Generate random string
        for (int i = 0; i < length; i++)
        {
            int charIndex = randomBytes[i] % chars.Length;
            sb.Append(chars[charIndex]);
        }

        return sb.ToString();
    }

    /// <inheritdoc />
    public string GenerateSecurePassword(int length, bool includeSpecialChars = true, bool includeNumbers = true, bool includeUppercase = true)
    {
        if (length < 8)
            throw new ArgumentException("Password length must be at least 8 characters", nameof(length));

        // Build character set based on requirements
        var charSet = new StringBuilder(LowercaseChars);
        
        if (includeUppercase)
            charSet.Append(UppercaseChars);
        
        if (includeNumbers)
            charSet.Append(NumericChars);
        
        if (includeSpecialChars)
            charSet.Append(SpecialChars);

        // Create random password
        byte[] randomBytes = GenerateRandomBytes(length * 2); // Get more bytes than needed to handle resampling
        char[] password = new char[length];
        
        // Ensure password includes at least one character from each required character set
        int position = 0;
        
        if (includeUppercase && position < length)
        {
            password[position] = UppercaseChars[randomBytes[position] % UppercaseChars.Length];
            position++;
        }
        
        if (includeNumbers && position < length)
        {
            password[position] = NumericChars[randomBytes[position] % NumericChars.Length];
            position++;
        }
        
        if (includeSpecialChars && position < length)
        {
            password[position] = SpecialChars[randomBytes[position] % SpecialChars.Length];
            position++;
        }
        
        // Always include at least one lowercase letter
        if (position < length)
        {
            password[position] = LowercaseChars[randomBytes[position] % LowercaseChars.Length];
            position++;
        }
        
        // Fill the rest with characters from the complete set
        string allPossibleChars = charSet.ToString();
        
        for (int i = position; i < length; i++)
        {
            password[i] = allPossibleChars[randomBytes[i] % allPossibleChars.Length];
        }
        
        // Shuffle the password characters
        for (int i = 0; i < length; i++)
        {
            int swapIndex = randomBytes[length + i] % length;
            (password[i], password[swapIndex]) = (password[swapIndex], password[i]);
        }
        
        return new string(password);
    }
} 