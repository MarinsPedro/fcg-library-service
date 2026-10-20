using FiapCloudGames.Library.Domain.Entries;
using FiapCloudGames.Library.Domain.Entries.Queries;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Library.Infrastructure.Persistence.Repositories;

internal sealed class LibraryEntryRepository(LibraryDbContext dbContext)
    : ILibraryEntryRepository
{
    public async Task AddAsync(
        LibraryEntry entry,
        CancellationToken cancellationToken) =>
        await dbContext.LibraryEntries.AddAsync(entry, cancellationToken);

    public Task<LibraryEntry?> GetAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        dbContext.LibraryEntries
            .AsNoTracking()
            .SingleOrDefaultAsync(entry => entry.Id == id, cancellationToken);

    public Task<LibraryEntry?> GetByOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken) =>
        dbContext.LibraryEntries
            .AsNoTracking()
            .SingleOrDefaultAsync(entry => entry.OrderId == orderId, cancellationToken);

    public Task<LibraryEntry?> GetByUserAndGameAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken) =>
        dbContext.LibraryEntries
            .AsNoTracking()
            .SingleOrDefaultAsync(
                entry => entry.UserId == userId && entry.GameId == gameId,
                cancellationToken);

    public Task<bool> ExistsAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken) =>
        dbContext.LibraryEntries
            .AsNoTracking()
            .AnyAsync(
                entry => entry.UserId == userId && entry.GameId == gameId,
                cancellationToken);

    public async Task<IReadOnlyList<LibraryEntry>> ListAsync(
        LibraryEntryQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var query = dbContext.LibraryEntries
            .AsNoTracking()
            .Where(entry => entry.UserId == parameters.UserId);

        if (parameters.BeforeGrantedAtUtc is not null && parameters.BeforeId is not null)
        {
            var grantedAtUtc = parameters.BeforeGrantedAtUtc.Value;
            var id = parameters.BeforeId.Value;

            query = query.Where(entry =>
                entry.GrantedAtUtc < grantedAtUtc ||
                (entry.GrantedAtUtc == grantedAtUtc && entry.Id.CompareTo(id) < 0));
        }

        return await query
            .OrderByDescending(entry => entry.GrantedAtUtc)
            .ThenByDescending(entry => entry.Id)
            .Take(parameters.Limit)
            .ToListAsync(cancellationToken);
    }
}
