using Vaultify.Domain.Exceptions;

namespace Vaultify.Domain.Entities;

public class PasswordEntry : BaseEntity
{
    public string Title { get; private set; }
    public string Username { get; private set; }
    public string EncryptedPassword { get; private set; }
    public string Website { get; private set; }
    public string Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastModified { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; }
    public bool IsFavorite { get; private set; }

    private PasswordEntry() { } // For EF Core

    public static PasswordEntry Create(
        string title,
        string username,
        string encryptedPassword,
        int categoryId,
        string website = null,
        string notes = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required");

        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("Username is required");

        if (string.IsNullOrWhiteSpace(encryptedPassword))
            throw new DomainException("Password is required");

        return new PasswordEntry
        {
            Title = title,
            Username = username,
            EncryptedPassword = encryptedPassword,
            CategoryId = categoryId,
            Website = website,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            IsFavorite = false
        };
    }

    public void Update(string title, string username, string website, string notes)
    {
        Title = !string.IsNullOrWhiteSpace(title) ? title : Title;
        Username = !string.IsNullOrWhiteSpace(username) ? username : Username;
        Website = website;
        Notes = notes;
        LastModified = DateTime.UtcNow;
    }

    public void UpdatePassword(string encryptedPassword)
    {
        if (string.IsNullOrWhiteSpace(encryptedPassword))
            throw new DomainException("Password is required");

        EncryptedPassword = encryptedPassword;
        LastModified = DateTime.UtcNow;
    }

    public void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
        LastModified = DateTime.UtcNow;
    }
}
