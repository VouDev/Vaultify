using Vaultify.Domain.Entities;

namespace Vaultify.Application.Interfaces;

public interface IPasswordRepository
{
    Task<PasswordEntry> GetByIdAsync(int id);
    Task<IEnumerable<PasswordEntry>> GetAllAsync();
    Task<IEnumerable<PasswordEntry>> GetByCategoryIdAsync(int categoryId);
    Task<int> AddAsync(PasswordEntry password);
    Task UpdateAsync(PasswordEntry password);
    Task DeleteAsync(int id);
}