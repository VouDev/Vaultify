using Microsoft.EntityFrameworkCore;
using Vaultify.Domain.Entities;

namespace Vaultify.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<PasswordVault> PasswordVaults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.Salt)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.LastLoginAt)
                .IsRequired();
                
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Navigation property configuration
            entity.HasMany(e => e.PasswordVaults)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PasswordVault>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.UserId)
                .IsRequired();
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.Website)
                .HasMaxLength(500);
                
            entity.Property(e => e.Notes)
                .HasMaxLength(1000);
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.LastAccessed)
                .IsRequired();
                
            entity.Property(e => e.LastModified)
                .IsRequired();

            // Navigation property configuration
            entity.HasOne(e => e.User)
                .WithMany(e => e.PasswordVaults)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
