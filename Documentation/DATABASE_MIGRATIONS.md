# Database Migrations Guide

## What is a Migration?
A migration is a version control system for your database schema. It:
- Represents changes to your database structure in code
- Tracks changes to your database model over time
- Enables easy updates or rollbacks of database changes
- Ensures database consistency across different environments

## Prerequisites

1. Install EF Core global tools:
   ```powershell
   # Install EF Core global tools
   dotnet tool install --global dotnet-ef
   
   # Or update if already installed
   dotnet tool update --global dotnet-ef
   ```

2. Required NuGet Packages in `Vaultify.Infrastructure.csproj`:
   ```xml
   <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.4" />
   <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.4" />
   <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.4" />
   <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.4" />
   ```

3. Required NuGet Packages in `Vaultify.API.csproj`:
   ```xml
   <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.4" />
   <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.4" />
   ```

## Migration Commands

### Creating a Migration
```powershell
# Create a new migration
dotnet ef migrations add <MigrationName> --project Vaultify.Infrastructure --startup-project Vaultify.API

# Example: Create initial migration
dotnet ef migrations add InitialCreate --project Vaultify.Infrastructure --startup-project Vaultify.API
```

### Applying Migrations
```powershell
# Apply all pending migrations
dotnet ef database update --project Vaultify.Infrastructure --startup-project Vaultify.API

# Apply to a specific migration
dotnet ef database update <MigrationName> --project Vaultify.Infrastructure --startup-project Vaultify.API
```

### Managing Migrations
```powershell
# List all migrations
dotnet ef migrations list --project Vaultify.Infrastructure --startup-project Vaultify.API

# Remove last migration (if not applied)
dotnet ef migrations remove --project Vaultify.Infrastructure --startup-project Vaultify.API

# Generate SQL script (to review changes)
dotnet ef migrations script --project Vaultify.Infrastructure --startup-project Vaultify.API
```

## When to Create New Migrations
Create a new migration when you:
1. Add a new entity
2. Modify existing entity properties
3. Change relationships between entities
4. Add or modify indexes or constraints

## Migration Files
When you create a migration, three files are generated in the `Migrations` folder:
1. `{timestamp}_{MigrationName}.cs` - The main migration file
2. `{timestamp}_{MigrationName}.Designer.cs` - Metadata about the migration
3. `ApplicationDbContextModelSnapshot.cs` - Current state of the database model

## Best Practices
1. Always create migrations for database changes
2. Test migrations in a development environment first
3. Keep migrations small and focused
4. Review generated SQL before applying to production
5. Use meaningful names for migrations
6. Document any manual steps required for the migration

## Troubleshooting
1. If migration fails, check:
   - All required packages are installed
   - Package versions are consistent
   - Connection string is correct
   - Database server is accessible

2. Common issues:
   - Package version mismatches
   - Missing design package
   - Incorrect startup project
   - Database connection issues

## Example Workflow
1. Make changes to your entities
2. Create a new migration:
   ```powershell
   dotnet ef migrations add AddNewFeature --project Vaultify.Infrastructure --startup-project Vaultify.API
   ```
3. Review the generated migration files
4. Apply the migration:
   ```powershell
   dotnet ef database update --project Vaultify.Infrastructure --startup-project Vaultify.API
   ```
5. Test the changes
6. Commit the migration files to version control 