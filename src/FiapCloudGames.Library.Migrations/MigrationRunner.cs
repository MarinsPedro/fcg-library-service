using FiapCloudGames.Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Library.Migrations;

internal sealed class MigrationRunner(
    LibraryDbContext dbContext,
    ILogger<MigrationRunner> logger)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            var pending = (await dbContext.Database
                .GetPendingMigrationsAsync(cancellationToken))
                .ToArray();

            if (pending.Length == 0)
            {
                logger.LogInformation("O banco do Library ja esta atualizado.");
                return;
            }

            logger.LogInformation(
                "Aplicando {MigrationCount} migration(s) do Library: {Migrations}.",
                pending.Length,
                string.Join(", ", pending));

            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Migrations do Library aplicadas com sucesso.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("A execucao das migrations foi cancelada.");
            throw;
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Falha ao aplicar as migrations do Library.");
            throw;
        }
    }
}
