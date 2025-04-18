using System.ComponentModel.DataAnnotations;

namespace Vaultify.Domain.Entities;

public class PasswordVault
{
    [Key]
    public Guid Id { get; private set; }

    [Required]
    public Guid UserId { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; private set; }

    [Required]
    [MaxLength(500)]
    public string Username { get; private set; }

    [Required]
    [MaxLength(500)]
    public string Password { get; private set; }

    [MaxLength(500)]
    public string Website { get; private set; }

    [MaxLength(1000)]
    public string Notes { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime LastAccessed { get; private set; }
    public DateTime LastModified { get; private set; }

    // Navigation property
    public User User { get; private set; }

    // Private constructor for EF Core
    private PasswordVault() 
    {
        Website = string.Empty;
        Notes = string.Empty;
    }

    public static PasswordVault Create(
        Guid userId,
        string name,
        string username,
        string password,
        string website = "",
        string notes = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        return new PasswordVault
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            Username = username,
            Password = password,
            Website = website ?? string.Empty,
            Notes = notes ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            LastAccessed = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string username,
        string password,
        string website = "",
        string notes = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        Name = name;
        Username = username;
        Password = password;
        Website = website ?? string.Empty;
        Notes = notes ?? string.Empty;
        LastModified = DateTime.UtcNow;
    }

    public void UpdateLastAccessed()
    {
        LastAccessed = DateTime.UtcNow;
    }
}
