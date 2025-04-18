# Vaultify

Vaultify is a secure, modern password manager application built with .NET 8 and React, following Clean Architecture principles.

## Features

- Secure password storage using industry-standard encryption
- Clean Architecture design with Domain-Driven Design
- .NET 8 Backend with FastEndpoints for high-performance API
- SQLite database with EF Core for data persistence
- Comprehensive test suite
- Modern, responsive UI built with React

## Project Structure

- **Vaultify.Domain**: Core entities, interfaces, and domain logic
- **Vaultify.Application**: Application services, CQRS handlers, and business logic
- **Vaultify.Infrastructure**: Data access, external services, and infrastructure concerns
- **Vaultify.API**: API endpoints, controllers, and configuration
- **Vaultify.Tests**: Integration and unit tests

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js and npm
- Visual Studio 2022 or later

### Setup

1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Run migrations to create the database

```bash
dotnet ef database update -p Vaultify.Infrastructure -s Vaultify.API
```

5. Run the application

## Development

- The application uses Entity Framework Core with SQLite
- Repository pattern is used for data access
- Authentication is handled securely with industry best practices

## License

This project is licensed under the MIT License - see the LICENSE file for details.