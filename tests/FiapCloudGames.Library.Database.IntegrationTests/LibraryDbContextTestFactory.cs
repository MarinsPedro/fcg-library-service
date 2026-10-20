using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Library.Migrations.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Library.Database.IntegrationTests;

internal static class LibraryDbContextTestFactory
{
    private const string ConnectionString =
        "Host=localhost;Database=model_only;Username=model;Password=model";

    public static LibraryDbContext Create()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>();
        MigrationDbContextOptions.Configure(options, ConnectionString);
        return new LibraryDbContext(options.Options);
    }
}
