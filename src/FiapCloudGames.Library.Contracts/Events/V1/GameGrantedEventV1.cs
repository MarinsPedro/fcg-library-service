
namespace FiapCloudGames.Library.Contracts.Events.V1;

public sealed record GameGrantedEventV1(
    Guid LibraryEntryId,
    Guid OrderId,
    Guid UserId,
    Guid GameId,
    DateTimeOffset GrantedAtUtc);
