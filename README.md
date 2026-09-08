# Movies API

Movies API is an ASP.NET Core Web API for managing movies and genres with Entity Framework Core and SQL Server. It separates HTTP endpoints from data-access services and uses DTOs to shape request and response data.

## What This Project Demonstrates

- Building and modifying ASP.NET Core API controllers
- Implementing asynchronous CRUD operations through service classes
- Modeling Movies and Genres with Entity Framework Core
- Persisting data in SQL Server with EF Core migrations
- Separating API contracts from database entities with DTOs
- Mapping between DTOs and models with AutoMapper
- Validating related records and uploaded poster files

## Technology Stack

- C# and .NET 6
- ASP.NET Core Web API
- Entity Framework Core 6
- SQL Server
- Swagger / OpenAPI through Swashbuckle
- AutoMapper

## API Structure

The confirmed API resources are:

- **Genres** — list, create, update, and delete genres
- **Movies** — list, retrieve by ID, create, update, and delete movies
- **Movie filtering** — list movies for a supplied genre
- **Poster upload** — accept JPG or PNG poster files up to 1 MB when creating or updating a movie

Movie responses include their related genre, and movie lists are ordered by rating in descending order.

## Project Structure

- `Controllers/` — HTTP endpoints for Movies and Genres
- `Dtos/` — request and response models for genre and movie data
- `Services/` — service interfaces and EF Core-backed implementations
- `Models/` — entity models and `ApplicationDbContext`
- `Helpers/` — AutoMapper configuration
- `Migrations/` — EF Core migrations for the Movies and Genres tables

## Run Locally

Prerequisites:

- .NET 6 SDK
- SQL Server

The application reads its database connection from `ConnectionStrings:DefaultConnection`. Configure a local value without committing credentials, for example:

```bash
dotnet user-secrets init --project MoviesAPI.csproj
dotnet user-secrets set --project MoviesAPI.csproj \
  "ConnectionStrings:DefaultConnection" "<local SQL Server connection string>"
```

Restore and build the solution:

```bash
dotnet restore MoviesAPI.sln
dotnet build MoviesAPI.sln
```

With the `dotnet-ef` tool available, apply the included migrations:

```bash
dotnet ef database update --project MoviesAPI.csproj
```

Run the API:

```bash
dotnet run --project MoviesAPI.csproj
```

In the Development environment, Swagger UI is available at `/swagger`.

## Freelance-Relevant Skills Demonstrated

This repository provides factual examples of ASP.NET Core API work, endpoint changes, EF Core queries and relationships, SQL-backed application changes, DTO-based API contracts, AutoMapper configuration, validation, and focused modifications across an existing backend structure.
