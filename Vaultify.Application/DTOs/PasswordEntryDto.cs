namespace Vaultify.Application.DTOs;

public class PasswordEntryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }  // Decrypted password for UI
    public string Website { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public bool IsFavorite { get; set; }
}

public class CreatePasswordEntryDto
{
    public string Title { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Website { get; set; }
    public string Notes { get; set; }
    public int CategoryId { get; set; }
}

public class UpdatePasswordEntryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Website { get; set; }
    public string Notes { get; set; }
    public int CategoryId { get; set; }
}
