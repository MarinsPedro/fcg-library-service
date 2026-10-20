using FiapCloudGames.Library.Domain.Entries.Queries;

namespace FiapCloudGames.Library.Domain.Entries;

public interface ILibraryEntryRepository
{
    Task AddAsync(LibraryEntry entry, CancellationToken cancellationToken);
    Task<LibraryEntry?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<LibraryEntry?> GetByOrderAsync(Guid orderId, CancellationToken cancellationToken);
    Task<LibraryEntry?> GetByUserAndGameAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
    Task<IReadOnlyList<LibraryEntry>> ListAsync(
        LibraryEntryQueryParameters parameters,
        CancellationToken cancellationToken);
}
