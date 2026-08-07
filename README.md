# Movies API

A modern ASP.NET Core Web API for managing movies and genres. The project is being modernized as a production-focused portfolio project with clear API contracts, validation, async EF Core access, Docker support, and CI.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server
- Swagger / OpenAPI
- Docker
- GitHub Actions

## Current Features

- List movies with optional genre filtering
- Get a movie by ID
- Create and update movies with poster validation
- Delete movies
- CRUD endpoints for genres
- DTO-based API responses
- Consistent HTTP status codes
- Configurable CORS policy
- Global Problem Details support
- Fully asynchronous database writes

## API Endpoints

### Movies

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/movies` | List movies |
| GET | `/api/movies?genreId={id}` | Filter movies by genre |
| GET | `/api/movies/{id}` | Get movie by ID |
| POST | `/api/movies` | Create movie |
| PUT | `/api/movies/{id}` | Update movie |
| DELETE | `/api/movies/{id}` | Delete movie |

### Genres

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/genres` | List genres |
| GET | `/api/genres/{id}` | Get genre by ID |
| POST | `/api/genres` | Create genre |
| PUT | `/api/genres/{id}` | Update genre |
| DELETE | `/api/genres/{id}` | Delete genre |

## Run Locally

### Prerequisites

- .NET 10 SDK
- SQL Server or SQL Server Express

Update `ConnectionStrings:DefaultConnection` in `appsettings.json` if your SQL Server instance is different.

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Swagger is available in the Development environment at `/swagger`.

## Docker

Build the image:

```bash
docker build -t movies-api .
```

Run it with an environment-specific SQL Server connection string:

```bash
docker run --rm -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING" \
  movies-api
```

## CI

GitHub Actions restores and builds the solution in Release mode on pushes and pull requests.

## Modernization Roadmap

Next priorities:

1. Add unit and integration tests.
2. Add pagination, search, and sorting.
3. Move poster storage behind a dedicated storage abstraction.
4. Add authentication and authorization only when protected write operations are required.
5. Add database-backed integration testing and deployment configuration.
