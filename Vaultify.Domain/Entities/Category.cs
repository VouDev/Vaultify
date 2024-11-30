using Vaultify.Domain.Exceptions;

namespace Vaultify.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string IconName { get; private set; }
    private readonly List<PasswordEntry> _passwords = new();
    public IReadOnlyCollection<PasswordEntry> Passwords => _passwords.AsReadOnly();

    private Category() { } // For EF Core

    public static Category Create(string name, string iconName = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required");

        return new Category
        {
            Name = name,
            IconName = iconName ?? "folder" // Default icon
        };
    }

    public void Update(string name, string iconName)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (!string.IsNullOrWhiteSpace(iconName))
            IconName = iconName;
    }
}