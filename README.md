# Employee Structure System

This repository contains the MVP foundation for an internal employee structure management tool built with ASP.NET Core MVC, Entity Framework Core, and SQL Server LocalDB.

## Solution Structure

- `EmployeeStructureSystem.Web` contains MVC controllers, view models, Razor views, and startup wiring.
- `EmployeeStructureSystem.Application` contains service contracts and shared operation result models.
- `EmployeeStructureSystem.Domain` contains entities and invariant enforcement.
- `EmployeeStructureSystem.Infrastructure` contains EF Core persistence, migrations, and service implementations.
- `EmployeeStructureSystem.Tests` contains focused validation and persistence behavior tests.

## Local Setup

1. Ensure `.NET SDK 9` and `SQL Server LocalDB` are installed.
2. Restore packages:
   `dotnet restore EmployeeStructureSystem.sln`
3. Apply the database schema:
   `dotnet dotnet-ef database update --project .\EmployeeStructureSystem.Infrastructure\EmployeeStructureSystem.Infrastructure.csproj --startup-project .\EmployeeStructureSystem.Web\EmployeeStructureSystem.Web.csproj`
4. Start the web application:
   `dotnet run --project .\EmployeeStructureSystem.Web\EmployeeStructureSystem.Web.csproj`

The default connection string targets:

`Server=(localdb)\MSSQLLocalDB;Database=EmployeeStructureSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`

## Migrations

- Create a migration:
  `dotnet dotnet-ef migrations add <MigrationName> --project .\EmployeeStructureSystem.Infrastructure\EmployeeStructureSystem.Infrastructure.csproj --startup-project .\EmployeeStructureSystem.Web\EmployeeStructureSystem.Web.csproj --output-dir Persistence\Migrations`
- Apply migrations:
  `dotnet dotnet-ef database update --project .\EmployeeStructureSystem.Infrastructure\EmployeeStructureSystem.Infrastructure.csproj --startup-project .\EmployeeStructureSystem.Web\EmployeeStructureSystem.Web.csproj`

## Current MVP Scope

- Dashboard with total departments and total employees.
- Department CRUD.
- Position CRUD.
- Employee entity and table prepared for later expansion.

Not included yet: employee CRUD UI, authentication, reporting, filtering, sorting, and exports.