using Vaultify.Application.DTOs;

namespace Vaultify.Application.Interfaces;

public interface IPasswordService
{
    Task<PasswordEntryDto> GetPasswordAsync(int id);
    Task<IEnumerable<PasswordEntryDto>> GetAllPasswordsAsync();
    Task<IEnumerable<PasswordEntryDto>> GetPasswordsByCategoryAsync(int categoryId);
    Task<int> CreatePasswordAsync(CreatePasswordEntryDto dto);
    Task UpdatePasswordAsync(UpdatePasswordEntryDto dto);
    Task DeletePasswordAsync(int id);
    Task ToggleFavoriteAsync(int id);
}