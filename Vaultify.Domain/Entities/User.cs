using System.ComponentModel.DataAnnotations;

namespace Vaultify.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; private set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; private set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; private set; }

    [Required]
    [MaxLength(255)]
    public string Salt { get; private set; }

    [Required]
    [MaxLength(255)]
    public string FirstName { get; private set; }

    [Required]
    [MaxLength(255)]
    public string LastName { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime LastLoginAt { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation property
    public ICollection<PasswordVault> PasswordVaults { get; private set; }

    // Private constructor for EF Core
    private User() 
    {
        PasswordVaults = [];
    }

    public static User Create(string email, string passwordHash, string salt, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt cannot be empty", nameof(salt));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            Salt = salt,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Updates the user's password
    /// </summary>
    /// <param name="passwordHash">The new password hash</param>
    /// <param name="salt">The new salt</param>
    /// <exception cref="ArgumentException">Thrown when password hash or salt is empty</exception>
    public void UpdatePassword(string passwordHash, string salt)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt cannot be empty", nameof(salt));
            
        PasswordHash = passwordHash;
        Salt = salt;
    }
}
