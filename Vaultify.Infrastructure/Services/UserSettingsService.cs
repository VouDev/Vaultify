using Microsoft.EntityFrameworkCore;
using Vaultify.Application.Interfaces;
using Vaultify.Infrastructure.Contexts;

namespace Vaultify.Infrastructure.Services;

public class UserSettingsService : IUserSettingsService
{
    // Assuming you have a DbContext named VaultifyDbContext
    private readonly VaultifyDbContext _context;

    public UserSettingsService(VaultifyDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetMasterKeyAsync()
    {
        var settings = await _context.UserSettings.FirstOrDefaultAsync();
        return settings?.HashedMasterPassword;
    }
}