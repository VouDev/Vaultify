using Microsoft.EntityFrameworkCore;
using Vaultify.Domain.Entities;

namespace Vaultify.Infrastructure.Contexts;

public class VaultifyDbContext : DbContext
{
    public VaultifyDbContext(DbContextOptions<VaultifyDbContext> options)
    : base(options)
    {
    }

    public DbSet<PasswordEntry> PasswordEntries { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity properties and relationships here
        modelBuilder.Entity<PasswordEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EncryptedPassword).IsRequired();
            entity.Property(e => e.Website).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.LastModified).IsRequired();

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Passwords)
                  .HasForeignKey(e => e.CategoryId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HashedMasterPassword).IsRequired();
            entity.Property(e => e.Salt).IsRequired();
            entity.Property(e => e.UseBiometrics).IsRequired();
            entity.Property(e => e.AutoLockEnabled).IsRequired();
            entity.Property(e => e.AutoLockTimeout).IsRequired();
            entity.Property(e => e.LastAccessTime).IsRequired();
        });
    }
}