using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Library.Domain.Entries;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Library.Application.Features.Entries.Get;

public sealed class GetLibraryEntryService(
    ILibraryEntryRepository repository,
    ILogger<GetLibraryEntryService> logger)
{
    public async Task<LibraryEntryResult> ExecuteAsync(
        Guid entryId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Localizando entrada da biblioteca com id {Id}.", entryId);

        var entry = await repository.GetAsync(entryId, cancellationToken)
            ?? throw AppException.NotFound("Entrada da biblioteca não encontrada.");

        if (entry.UserId != userId)
            throw AppException.NotFound($"Entrada da biblioteca para o usuário {userId} não encontrada.");

        return LibraryApplicationMappings.ToResult(entry);
    }
}
