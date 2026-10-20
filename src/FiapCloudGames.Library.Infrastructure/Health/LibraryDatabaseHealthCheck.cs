using FiapCloudGames.Library.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FiapCloudGames.Library.Infrastructure.Health;

internal sealed class LibraryDatabaseHealthCheck(IServiceScopeFactory scopeFactory)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

            return await database.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy("PostgreSQL acessivel.")
                : HealthCheckResult.Unhealthy("PostgreSQL indisponivel.");
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy(
                "Falha ao verificar o PostgreSQL.",
                exception);
        }
    }
}
