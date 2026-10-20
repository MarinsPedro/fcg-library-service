using FiapCloudGames.Library.Application.Abstractions.Persistence;
using FiapCloudGames.Library.Domain.Entries;
using FiapCloudGames.Library.Domain.Entries.Queries;

namespace FiapCloudGames.Library.UnitTests.Support;

internal sealed class FakeLibraryEntryRepository : ILibraryEntryRepository
{
    internal List<LibraryEntry> Entries { get; } = [];
    internal LibraryEntry? AddedEntry { get; private set; }

    public Task AddAsync(LibraryEntry entry, CancellationToken cancellationToken)
    {
        AddedEntry = entry;
        Entries.Add(entry);
        return Task.CompletedTask;
    }

    public Task<LibraryEntry?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Entries.SingleOrDefault(entry => entry.Id == id));

    public Task<LibraryEntry?> GetByOrderAsync(Guid orderId, CancellationToken cancellationToken) =>
        Task.FromResult(Entries.SingleOrDefault(entry => entry.OrderId == orderId));

    public Task<LibraryEntry?> GetByUserAndGameAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken) =>
        Task.FromResult(Entries.SingleOrDefault(
            entry => entry.UserId == userId && entry.GameId == gameId));

    public Task<bool> ExistsAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken) =>
        Task.FromResult(Entries.Any(
            entry => entry.UserId == userId && entry.GameId == gameId));

    public Task<IReadOnlyList<LibraryEntry>> ListAsync(
        LibraryEntryQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        IEnumerable<LibraryEntry> query = Entries
            .Where(entry => entry.UserId == parameters.UserId)
            .OrderByDescending(entry => entry.GrantedAtUtc)
            .ThenByDescending(entry => entry.Id);

        if (parameters.BeforeGrantedAtUtc is not null && parameters.BeforeId is not null)
        {
            var time = parameters.BeforeGrantedAtUtc.Value;
            var id = parameters.BeforeId.Value;
            query = query.Where(entry =>
                entry.GrantedAtUtc < time ||
                (entry.GrantedAtUtc == time && entry.Id.CompareTo(id) < 0));
        }

        return Task.FromResult<IReadOnlyList<LibraryEntry>>(
            query.Take(parameters.Limit).ToArray());
    }
}

internal sealed class SpyLibraryUnitOfWork : ILibraryUnitOfWork
{
    internal int SaveChangesCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}

internal static class LibraryTestData
{
    internal static readonly DateTimeOffset NowUtc =
        new(2026, 10, 19, 15, 0, 0, TimeSpan.Zero);

    internal static readonly Guid UserId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    internal static readonly Guid GameId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    internal static readonly Guid OrderId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");
}
