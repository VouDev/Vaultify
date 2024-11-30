namespace Vaultify.Domain.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText, string key);
    string Decrypt(string cipherText, string key);
    string HashPassword(string password, string salt);
    bool VerifyPassword(string password, string hash, string salt);
}
