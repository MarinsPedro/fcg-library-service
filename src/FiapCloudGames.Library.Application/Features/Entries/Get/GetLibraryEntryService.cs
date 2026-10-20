using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Library.Domain.Entries;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Library.Application.Features.Entries.Get;

public sealed class GetLibraryEntryService(
    ILibraryEntryRepository repository,
    ILogger<GetLibraryEntryService> logger)
{
    public async Task<LibraryEntryResult> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Localizando entrada da biblioteca com id {Id}.", id);

        var entry = await repository.GetAsync(id, cancellationToken)
            ?? throw AppException.NotFound("Entrada da biblioteca nao encontrada.");

        return LibraryApplicationMappings.ToResult(entry);
    }
}
