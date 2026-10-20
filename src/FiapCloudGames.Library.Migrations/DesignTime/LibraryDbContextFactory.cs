using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Library.Migrations.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiapCloudGames.Library.Migrations.DesignTime;

public sealed class LibraryDbContextFactory
    : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var builder = MigrationConnection.CreateBuilder(args);
        var options = new DbContextOptionsBuilder<LibraryDbContext>();

        MigrationDbContextOptions.Configure(
            options,
            MigrationConnection.GetRequired(builder.Configuration));

        return new LibraryDbContext(options.Options);
    }
}
