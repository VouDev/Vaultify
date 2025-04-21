using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vaultify.Application.Interfaces.Services.Users;
using Vaultify.Application.Interfaces.Services.Vaults;
using Vaultify.Application.Services.Users;
using Vaultify.Application.Services.Vaults;
using Vaultify.Domain.Interfaces.Repositories;
using Vaultify.Domain.Interfaces.Security;
using Vaultify.Infrastructure.Data;
using Vaultify.Infrastructure.Repositories;
using Vaultify.Infrastructure.Security;

namespace Vaultify.Infrastructure;

/// <summary>
/// Extension methods for setting up infrastructure services in an IServiceCollection
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the specified IServiceCollection
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? 
                              "Data Source=Vaultify.db"));

        // Register repositories
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordVaultRepository, PasswordVaultRepository>();
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Register security services
        services.AddSingleton<ISecureRandomGenerator, SecureRandomGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IEncryptionService, EncryptionService>();
        
        // Register application services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPasswordVaultService, PasswordVaultService>();

        // Add default logging
        services.AddLogging();

        return services;
    }
} 