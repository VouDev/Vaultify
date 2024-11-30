using Vaultify.Application.DTOs;
using Vaultify.Application.Interfaces;
using Vaultify.Domain.Entities;

namespace Vaultify.Application.Services;

public class PasswordService : IPasswordService
{
    private readonly IPasswordRepository _passwordRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IUserSettingsService _userSettingsService;

    public PasswordService(
        IPasswordRepository passwordRepository,
        IEncryptionService encryptionService,
        IUserSettingsService userSettingsService)
    {
        _passwordRepository = passwordRepository;
        _encryptionService = encryptionService;
        _userSettingsService = userSettingsService;
    }

    public async Task<PasswordEntryDto> GetPasswordAsync(int id)
    {
        var entry = await _passwordRepository.GetByIdAsync(id);
        if (entry == null) return null;

        var masterKey = await _userSettingsService.GetMasterKeyAsync();
        return new PasswordEntryDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Username = entry.Username,
            Password = _encryptionService.Decrypt(entry.EncryptedPassword, masterKey),
            Website = entry.Website,
            Notes = entry.Notes,
            CreatedAt = entry.CreatedAt,
            LastModified = entry.LastModified,
            CategoryId = entry.CategoryId,
            CategoryName = entry.Category?.Name,
            IsFavorite = entry.IsFavorite
        };
    }

    public async Task<int> CreatePasswordAsync(CreatePasswordEntryDto dto)
    {
        var masterKey = await _userSettingsService.GetMasterKeyAsync();
        var encryptedPassword = _encryptionService.Encrypt(dto.Password, masterKey);

        var entry = PasswordEntry.Create(
            dto.Title,
            dto.Username,
            encryptedPassword,
            dto.CategoryId,
            dto.Website,
            dto.Notes
        );

        return await _passwordRepository.AddAsync(entry);
    }

    public async Task UpdatePasswordAsync(UpdatePasswordEntryDto dto)
    {
        var entry = await _passwordRepository.GetByIdAsync(dto.Id);
        if (entry == null) throw new KeyNotFoundException("Password entry not found");

        var masterKey = await _userSettingsService.GetMasterKeyAsync();
        var encryptedPassword = _encryptionService.Encrypt(dto.Password, masterKey);

        entry.Update(dto.Title, dto.Username, dto.Website, dto.Notes);
        entry.UpdatePassword(encryptedPassword);

        await _passwordRepository.UpdateAsync(entry);
    }

    public async Task DeletePasswordAsync(int id)
    {
        await _passwordRepository.DeleteAsync(id);
    }

    public async Task ToggleFavoriteAsync(int id)
    {
        var entry = await _passwordRepository.GetByIdAsync(id);
        if (entry == null) throw new KeyNotFoundException("Password entry not found");

        entry.ToggleFavorite();
        await _passwordRepository.UpdateAsync(entry);
    }

    public async Task<IEnumerable<PasswordEntryDto>> GetAllPasswordsAsync()
    {
        var entries = await _passwordRepository.GetAllAsync();
        var masterKey = await _userSettingsService.GetMasterKeyAsync();

        return entries.Select(entry => new PasswordEntryDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Username = entry.Username,
            Password = _encryptionService.Decrypt(entry.EncryptedPassword, masterKey),
            Website = entry.Website,
            Notes = entry.Notes,
            CreatedAt = entry.CreatedAt,
            LastModified = entry.LastModified,
            CategoryId = entry.CategoryId,
            CategoryName = entry.Category?.Name,
            IsFavorite = entry.IsFavorite
        });
    }

    public async Task<IEnumerable<PasswordEntryDto>> GetPasswordsByCategoryAsync(int categoryId)
    {
        var entries = await _passwordRepository.GetByCategoryIdAsync(categoryId);
        var masterKey = await _userSettingsService.GetMasterKeyAsync();

        return entries.Select(entry => new PasswordEntryDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Username = entry.Username,
            Password = _encryptionService.Decrypt(entry.EncryptedPassword, masterKey),
            Website = entry.Website,
            Notes = entry.Notes,
            CreatedAt = entry.CreatedAt,
            LastModified = entry.LastModified,
            CategoryId = entry.CategoryId,
            CategoryName = entry.Category?.Name,
            IsFavorite = entry.IsFavorite
        });
    }
}