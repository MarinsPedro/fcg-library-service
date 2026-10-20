using FiapCloudGames.Library.Application;
using FiapCloudGames.Library.Application.Abstractions.Persistence;
using FiapCloudGames.Library.Domain.Entries;
using FiapCloudGames.Library.Infrastructure.Health;
using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Library.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Library.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLibraryInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "A connection string 'Database' nao foi configurada.");

        services.AddLibraryApplication();
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ILibraryUnitOfWork>(provider =>
            provider.GetRequiredService<LibraryDbContext>());
        services.AddScoped<ILibraryEntryRepository, LibraryEntryRepository>();

        services.AddHealthChecks()
            .AddCheck<LibraryDatabaseHealthCheck>(
                "library-database",
                tags: ["ready"]);

        return services;
    }
}
