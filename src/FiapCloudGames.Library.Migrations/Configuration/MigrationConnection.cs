using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FiapCloudGames.Library.Migrations.Configuration;

internal static class MigrationConnection
{
    private static readonly string[] OverrideVariableNames =
    [
        "LIBRARY_MIGRATIONS_CONNECTION",
        "ConnectionStrings__Database",
        "LIBRARY_DESIGN_CONNECTION"
    ];

    public static HostApplicationBuilder CreateBuilder(string[] args) =>
        Host.CreateApplicationBuilder(
            new HostApplicationBuilderSettings
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory
            });

    public static string GetRequired(IConfiguration configuration)
    {
        foreach (var variableName in OverrideVariableNames)
        {
            var value = Environment.GetEnvironmentVariable(variableName);
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        var configuredConnection = configuration.GetConnectionString("Database");
        if (!string.IsNullOrWhiteSpace(configuredConnection))
            return configuredConnection;

        throw new InvalidOperationException(
            "A connection string 'Database' nao foi configurada. " +
            "Configure-a no appsettings.json ou por variavel de ambiente.");
    }
}
