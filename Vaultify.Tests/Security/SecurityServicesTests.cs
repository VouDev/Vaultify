using Vaultify.Domain.Interfaces.Security;
using Vaultify.Infrastructure.Security;
using Xunit;

namespace Vaultify.Tests.Security;

public class SecurityServicesTests
{
    private readonly ISecureRandomGenerator _randomGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEncryptionService _encryptionService;

    public SecurityServicesTests()
    {
        _randomGenerator = new SecureRandomGenerator();
        _passwordHasher = new PasswordHasher();
        _encryptionService = new EncryptionService(_randomGenerator);
    }

    [Fact]
    public void PasswordHasher_ShouldGenerateAndVerifyHashes()
    {
        // Arrange
        string password = "MySecurePassword123!";
        
        // Act
        (string hash, string salt) = _passwordHasher.HashPassword(password);
        bool isPasswordValid = _passwordHasher.VerifyPassword(password, hash, salt);
        bool isWrongPasswordValid = _passwordHasher.VerifyPassword("WrongPassword", hash, salt);
        
        // Assert
        Assert.NotEmpty(hash);
        Assert.NotEmpty(salt);
        Assert.True(isPasswordValid);
        Assert.False(isWrongPasswordValid);
    }

    [Fact]
    public void EncryptionService_ShouldEncryptAndDecryptData()
    {
        // Arrange
        string originalText = "This is my sensitive data that needs to be encrypted";
        string masterKey = _encryptionService.GenerateKey();
        
        // Act
        string encryptedText = _encryptionService.Encrypt(originalText, masterKey);
        string decryptedText = _encryptionService.Decrypt(encryptedText, masterKey);
        
        // Assert
        Assert.NotEmpty(encryptedText);
        Assert.NotEqual(originalText, encryptedText);
        Assert.Equal(originalText, decryptedText);
    }
    
    [Fact]
    public void EncryptionService_ShouldFailWithWrongKey()
    {
        // Arrange
        string originalText = "This is my sensitive data that needs to be encrypted";
        string masterKey = _encryptionService.GenerateKey();
        string wrongKey = _encryptionService.GenerateKey();
        
        // Act
        string encryptedText = _encryptionService.Encrypt(originalText, masterKey);
        
        // Assert
        Assert.Throws<InvalidOperationException>(() => 
            _encryptionService.Decrypt(encryptedText, wrongKey));
    }

    [Fact]
    public void RandomGenerator_ShouldGenerateSecurePasswords()
    {
        // Act
        string password1 = _randomGenerator.GenerateSecurePassword(12);
        string password2 = _randomGenerator.GenerateSecurePassword(12);
        
        // Assert
        Assert.Equal(12, password1.Length);
        Assert.NotEqual(password1, password2); // Passwords should be different
        
        // Verify password contains required character types
        Assert.Contains(password1, c => char.IsUpper(c));
        Assert.Contains(password1, c => char.IsLower(c));
        Assert.Contains(password1, c => char.IsDigit(c));
        Assert.Contains(password1, c => !char.IsLetterOrDigit(c));
    }
} 