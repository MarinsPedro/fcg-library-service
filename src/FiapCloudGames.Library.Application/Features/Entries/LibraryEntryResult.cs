namespace FiapCloudGames.Library.Application.Features.Entries;

public sealed record LibraryEntryResult(
    Guid Id,
    Guid UserId,
    Guid GameId,
    Guid OrderId,
    DateTimeOffset GrantedAtUtc);
