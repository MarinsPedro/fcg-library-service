using FiapCloudGames.Library.Domain.Entries;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Library.Application.Features.Entries.CheckOwnership;

public sealed class CheckGameOwnershipService(
    ILibraryEntryRepository repository,
    ILogger<CheckGameOwnershipService> logger)
{
    public async Task<CheckGameOwnershipResult> ExecuteAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Verificando propriedade do jogo. UserId={UserId}, GameId={GameId}.",
            userId,
            gameId);

        var owned = await repository.ExistsAsync(
            userId,
            gameId,
            cancellationToken);

        return new CheckGameOwnershipResult(userId, gameId, owned);
    }
}
