using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Library.Application.Abstractions.Persistence;
using FiapCloudGames.Library.Domain.Entries;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Library.Application.Features.Entries.Grant;

public sealed class GrantGameService(
    ILibraryEntryRepository repository,
    ILibraryUnitOfWork unitOfWork,
    ILogger<GrantGameService> logger)
{
    public async Task<LibraryEntryResult> ExecuteAsync(
        GrantGameInput input,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Concedendo jogo para a biblioteca. OrderId={OrderId}, UserId={UserId}, GameId={GameId}.",
            input.OrderId,
            input.UserId,
            input.GameId);

        var existingOrder = await repository.GetByOrderAsync(
            input.OrderId,
            cancellationToken);

        if (existingOrder is not null)
        {
            if (existingOrder.UserId != input.UserId ||
                existingOrder.GameId != input.GameId)
            {
                throw AppException.Conflict(
                    "O pedido ja foi processado com dados de ownership diferentes.");
            }

            logger.LogInformation(
                "Pedido {OrderId} ja foi processado pela biblioteca.",
                input.OrderId);

            return LibraryApplicationMappings.ToResult(existingOrder);
        }

        var existingOwnership = await repository.GetByUserAndGameAsync(
            input.UserId,
            input.GameId,
            cancellationToken);

        if (existingOwnership is not null)
        {
            throw AppException.Conflict(
                "O usuario ja possui este jogo por outro pedido.");
        }

        var entry = LibraryEntry.Create(
            input.UserId,
            input.GameId,
            input.OrderId,
            input.OccurredAtUtc);

        await repository.AddAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Jogo concedido com sucesso. LibraryEntryId={LibraryEntryId}.",
            entry.Id);

        return LibraryApplicationMappings.ToResult(entry);
    }
}
