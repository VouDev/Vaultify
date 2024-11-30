using Vaultify.Domain.Entities;

namespace Vaultify.Domain.Interfaces;

public interface IPasswordRepository
{
    Task<PasswordEntry> GetByIdAsync(int id);
    Task<IEnumerable<PasswordEntry>> GetAllAsync();
    Task<int> AddAsync(PasswordEntry password);
    Task UpdateAsync(PasswordEntry password);
    Task DeleteAsync(int id);
}
