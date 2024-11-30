namespace Vaultify.Application.Interfaces;

public interface IUserSettingsService
{
    Task<string> GetMasterKeyAsync();
}