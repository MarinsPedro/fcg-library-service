namespace FiapCloudGames.Library.Domain.Entries.Queries;

public sealed class LibraryEntryQueryParameters
{
    public required Guid UserId { get; init; }
    public DateTimeOffset? BeforeGrantedAtUtc { get; init; }
    public Guid? BeforeId { get; init; }
    public int Limit { get; init; }
}
