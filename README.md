# Movies API

A modern ASP.NET Core Web API for managing movies and genres. The project has been modernized as a production-focused portfolio project with clear API contracts, validation, asynchronous EF Core access, pagination, search, sorting, integration tests, Docker support, and CI.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server
- SQLite in-memory for integration tests
- MSTest / Microsoft.Testing.Platform
- Swagger / OpenAPI
- Docker
- GitHub Actions

## Current Features

- Paginated movie listing with bounded page sizes
- Movie search across title and storyline
- Genre filtering
- Controlled sorting by rate, title, or year
- Get a movie by ID
- Create and update movies with poster validation
- Delete movies
- CRUD endpoints for genres
- DTO-based API responses
- Consistent HTTP status codes and named routes for generated resource URLs
- Configurable CORS policy
- Global Problem Details support
- Fully asynchronous database writes
- Read-only EF Core queries using `AsNoTracking`
- Integration tests that exercise the real HTTP pipeline against a relational in-memory database
- Automated restore, build, and test validation in GitHub Actions
- CI concurrency that cancels superseded pull-request runs

## API Endpoints

### Movies

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/movies` | Paginated movie list |
| GET | `/api/movies/{id}` | Get movie by ID |
| POST | `/api/movies` | Create movie |
| PUT | `/api/movies/{id}` | Update movie |
| DELETE | `/api/movies/{id}` | Delete movie |

### Movie Query Parameters

`GET /api/movies` accepts:

| Parameter | Default | Rules |
| --- | --- | --- |
| `genreId` | none | Optional genre filter |
| `search` | none | Searches title and storyline, max 100 characters |
| `sortBy` | `Rate` | `Rate`, `Title`, or `Year` |
| `sortDirection` | `Desc` | `Asc` or `Desc` |
| `page` | `1` | Minimum 1 |
| `pageSize` | `10` | 1-50 |

Example:

```http
GET /api/movies?search=matrix&sortBy=Title&sortDirection=Asc&page=1&pageSize=10
```

The response is an envelope containing `items`, `page`, `pageSize`, `totalCount`, and `totalPages`.

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

## Tests

The integration test suite boots the ASP.NET Core application through `WebApplicationFactory`, replaces SQL Server with an in-memory SQLite relational database, seeds deterministic test data, and sends HTTP requests through the real API pipeline.

Run the tests with:

```bash
dotnet test MoviesAPI.Tests/MoviesAPI.Tests.csproj --configuration Release
```

The current suite contains 10 integration tests covering:

- movie and genre reads
- pagination metadata
- search
- genre filtering
- sorting and page selection
- invalid page-size validation
- successful resource creation and generated locations
- invalid movie genre validation
- not-found delete behavior

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

GitHub Actions restores, builds, and runs the integration test suite for pull requests targeting `master` and for pushes to `master`. Superseded CI runs for the same pull request are cancelled automatically.

## Modernization Roadmap

Next priorities:

1. Move poster persistence behind a dedicated storage abstraction instead of storing image bytes with the movie record.
2. Avoid returning poster payloads as part of movie list responses and expose media through a dedicated endpoint or external object storage.
3. Add authentication and authorization when write operations need to be protected.
4. Add deployment configuration and an environment-specific database migration strategy.
