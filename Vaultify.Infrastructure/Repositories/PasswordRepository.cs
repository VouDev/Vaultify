using Microsoft.EntityFrameworkCore;
using Vaultify.Domain.Entities;
using Vaultify.Application.Interfaces;
using Vaultify.Infrastructure.Contexts;

namespace Vaultify.Infrastructure.Repositories;

public class PasswordRepository : IPasswordRepository
{
    private readonly VaultifyDbContext _context;

    public PasswordRepository(VaultifyDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordEntry> GetByIdAsync(int id)
    {
        return await _context.PasswordEntries.FindAsync(id);
    }

    public async Task<IEnumerable<PasswordEntry>> GetAllAsync()
    {
        return await _context.PasswordEntries.ToListAsync();
    }

    public async Task<IEnumerable<PasswordEntry>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.PasswordEntries.Where(p => p.CategoryId == categoryId).ToListAsync();
    }

    public async Task<int> AddAsync(PasswordEntry password)
    {
        _context.PasswordEntries.Add(password);
        return await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PasswordEntry password)
    {
        _context.PasswordEntries.Update(password);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entry = await _context.PasswordEntries.FindAsync(id);
        _context.PasswordEntries.Remove(entry);
        await _context.SaveChangesAsync();
    }
}