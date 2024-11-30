using Vaultify.Domain.Exceptions;

namespace Vaultify.Domain.Entities;

public class UserSettings : BaseEntity
{
    public string HashedMasterPassword { get; private set; }
    public string Salt { get; private set; }
    public bool UseBiometrics { get; private set; }
    public bool AutoLockEnabled { get; private set; }
    public int AutoLockTimeout { get; private set; }
    public DateTime LastAccessTime { get; private set; }

    private UserSettings() { } // For EF Core

    public static UserSettings Create(string hashedPassword, string salt)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new DomainException("Master password is required");

        if (string.IsNullOrWhiteSpace(salt))
            throw new DomainException("Salt is required");

        return new UserSettings
        {
            HashedMasterPassword = hashedPassword,
            Salt = salt,
            UseBiometrics = false,
            AutoLockEnabled = true,
            AutoLockTimeout = 5, // Default 5 minutes
            LastAccessTime = DateTime.UtcNow
        };
    }

    public void UpdateSecuritySettings(bool useBiometrics, bool autoLockEnabled, int autoLockTimeout)
    {
        UseBiometrics = useBiometrics;
        AutoLockEnabled = autoLockEnabled;
        AutoLockTimeout = autoLockTimeout > 0 ? autoLockTimeout : throw new DomainException("Timeout must be greater than 0");
    }

    public void UpdateMasterPassword(string newHashedPassword, string newSalt)
    {
        if (string.IsNullOrWhiteSpace(newHashedPassword) || string.IsNullOrWhiteSpace(newSalt))
            throw new DomainException("Invalid password or salt");

        HashedMasterPassword = newHashedPassword;
        Salt = newSalt;
    }

    public void UpdateLastAccessTime()
    {
        LastAccessTime = DateTime.UtcNow;
    }
}
