namespace FiapCloudGames.Library.Presentation.Features.Entries;

public sealed record LibraryEntryResponse(
    Guid Id,
    Guid UserId,
    Guid GameId,
    Guid OrderId,
    DateTimeOffset GrantedAtUtc);
