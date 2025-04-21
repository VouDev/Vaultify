using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Vaultify.Domain.Entities;
using Vaultify.Domain.Interfaces.Repositories;
using Vaultify.Domain.Interfaces.Security;
using Vaultify.Domain.Interfaces.Services.Users;
using Vaultify.Infrastructure.Data;
using Vaultify.Infrastructure.Repositories;
using Vaultify.Infrastructure.Security;
using Vaultify.Infrastructure.Services.Users;
using Xunit;

namespace Vaultify.Tests.Services;

public class UserServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserService _userService;
    private readonly UserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        // Create mock logger
        var loggerMock = new Mock<ILogger<UserService>>();
        _logger = loggerMock.Object;

        // Create real repositories and passwordHasher
        _passwordHasher = new PasswordHasher();
        _userRepository = new UserRepository(_context);
        var passwordVaultRepository = new PasswordVaultRepository(_context);

        // Create unit of work
        _unitOfWork = new UnitOfWork(_context, _userRepository, passwordVaultRepository);
        
        // Initialize service with unit of work
        _userService = new UserService(_unitOfWork, _passwordHasher, _logger);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateUser_ShouldCreateNewUser()
    {
        // Arrange
        string email = "test@example.com";
        string password = "StrongPassword123!";
        string firstName = "John";
        string lastName = "Doe";

        // Act
        var user = await _userService.CreateUserAsync(email, password, firstName, lastName);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotEmpty(user.Salt);
        
        // Verify user is in database
        var dbUser = await _userRepository.GetByEmailAsync(email, CancellationToken.None);
        Assert.NotNull(dbUser);
        Assert.Equal(user.Id, dbUser.Id);
    }

    [Fact]
    public async Task CreateUser_ShouldThrowException_WhenEmailIsAlreadyInUse()
    {
        // Arrange
        string email = "duplicate@example.com";
        string password = "StrongPassword123!";
        string firstName = "John";
        string lastName = "Doe";

        // Create user first time
        await _userService.CreateUserAsync(email, password, firstName, lastName);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _userService.CreateUserAsync(email, "DifferentPassword123!", firstName, lastName));
    }

    [Fact]
    public async Task Authenticate_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange
        string email = "auth@example.com";
        string password = "AuthPassword123!";
        string firstName = "Jane";
        string lastName = "Smith";

        await _userService.CreateUserAsync(email, password, firstName, lastName);

        // Act
        var authenticatedUser = await _userService.AuthenticateAsync(email, password);

        // Assert
        Assert.NotNull(authenticatedUser);
        Assert.Equal(email, authenticatedUser.Email);
    }

    [Fact]
    public async Task Authenticate_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        string email = "wrong@example.com";
        string password = "WrongPassword123!";
        string firstName = "Wrong";
        string lastName = "User";

        await _userService.CreateUserAsync(email, password, firstName, lastName);

        // Act
        var result = await _userService.AuthenticateAsync(email, "IncorrectPassword123!");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ChangePassword_ShouldUpdatePassword_WhenCurrentPasswordIsCorrect()
    {
        // Arrange
        string email = "changepw@example.com";
        string currentPassword = "CurrentPassword123!";
        string newPassword = "NewPassword456!";
        string firstName = "Change";
        string lastName = "Password";

        var user = await _userService.CreateUserAsync(email, currentPassword, firstName, lastName);

        // Act
        bool result = await _userService.ChangePasswordAsync(user.Id, currentPassword, newPassword);

        // Assert
        Assert.True(result);
        
        // Verify old password no longer works
        var authWithOldPw = await _userService.AuthenticateAsync(email, currentPassword);
        Assert.Null(authWithOldPw);
        
        // Verify new password works
        var authWithNewPw = await _userService.AuthenticateAsync(email, newPassword);
        Assert.NotNull(authWithNewPw);
    }

    [Fact]
    public async Task UpdateUserProfile_ShouldUpdateNameFields()
    {
        // Arrange
        string email = "profile@example.com";
        string password = "ProfilePassword123!";
        string firstName = "Original";
        string lastName = "Name";
        string newFirstName = "Updated";
        string newLastName = "Profile";

        var user = await _userService.CreateUserAsync(email, password, firstName, lastName);

        // Act
        var updatedUser = await _userService.UpdateUserProfileAsync(user.Id, newFirstName, newLastName);

        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal(newFirstName, updatedUser.FirstName);
        Assert.Equal(newLastName, updatedUser.LastName);
        
        // Verify changes are persisted
        var dbUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.Equal(newFirstName, dbUser.FirstName);
        Assert.Equal(newLastName, dbUser.LastName);
    }
} 