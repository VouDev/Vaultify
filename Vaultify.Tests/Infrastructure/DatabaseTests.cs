using Microsoft.EntityFrameworkCore;
using Vaultify.Domain.Entities;
using Vaultify.Infrastructure.Data;
using Vaultify.Infrastructure.Repositories;
using Xunit;

namespace Vaultify.Tests.Infrastructure;

public class DatabaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _userRepository;

    public DatabaseTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _userRepository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [Fact]
    public async Task CanCreateAndRetrieveUser()
    {
        // Arrange
        var user = User.Create(
            "test@example.com",
            "hashed_password",
            "salt",
            "Test",
            "User"
        );

        // Act
        await _userRepository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var retrievedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.NotNull(retrievedUser);
        Assert.Equal(user.Email, retrievedUser.Email);
        Assert.Equal(user.FirstName, retrievedUser.FirstName);
        Assert.Equal(user.LastName, retrievedUser.LastName);
    }

    [Fact]
    public async Task CanUpdateUser()
    {
        // Arrange
        var user = User.Create(
            "test@example.com",
            "hashed_password",
            "salt",
            "Test",
            "User"
        );

        await _userRepository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        user.UpdateName("Updated", "User");
        _userRepository.Update(user);
        await _context.SaveChangesAsync();

        // Assert
        var updatedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.FirstName);
    }

    [Fact]
    public async Task CanDeleteUser()
    {
        // Arrange
        var user = User.Create(
            "test@example.com",
            "hashed_password",
            "salt",
            "Test",
            "User"
        );

        await _userRepository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        _userRepository.Remove(user);
        await _context.SaveChangesAsync();

        // Assert
        var deletedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task CanGetUserByEmail()
    {
        // Arrange
        var user = User.Create(
            "test@example.com",
            "hashed_password",
            "salt",
            "Test",
            "User"
        );

        await _userRepository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var retrievedUser = await _userRepository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal(user.Email, retrievedUser.Email);
    }
} 