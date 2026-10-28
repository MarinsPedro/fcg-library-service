
namespace FiapCloudGames.Library.Contracts.Events.V1;

public sealed record GameGrantRequestedEventV1(
    Guid OrderId,
    Guid UserId,
    Guid GameId,
    DateTimeOffset OccurredAtUtc);
