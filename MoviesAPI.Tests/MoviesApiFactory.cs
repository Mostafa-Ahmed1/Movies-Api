using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesAPI.Models;

namespace MoviesAPI.Tests;

public sealed class MoviesApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbContextConfiguration = services.SingleOrDefault(descriptor =>
                descriptor.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));

            if (dbContextConfiguration is not null)
                services.Remove(dbContextConfiguration);

            var dbConnection = services.SingleOrDefault(descriptor =>
                descriptor.ServiceType == typeof(DbConnection));

            if (dbConnection is not null)
                services.Remove(dbConnection);

            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection("Data Source=:memory:");
                connection.Open();
                return connection;
            });

            services.AddDbContext<ApplicationDbContext>((provider, options) =>
                options.UseSqlite(provider.GetRequiredService<DbConnection>()));

            services.AddHostedService<TestDatabaseInitializer>();
        });
    }
}

internal sealed class TestDatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public TestDatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Genres.AnyAsync(cancellationToken))
            return;

        var action = new Genre { Name = "Action" };
        var drama = new Genre { Name = "Drama" };

        dbContext.Genres.AddRange(action, drama);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Movies.Add(new Movie
        {
            Title = "The Matrix",
            Year = 1999,
            Rate = 8.7,
            Storeline = "A hacker discovers the world is a simulated reality.",
            Poster = new byte[] { 1, 2, 3 },
            GenreId = action.Id,
            Genre = action
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
