namespace FiapCloudGames.Library.Contracts.Events;

public sealed record OrderCompletedEvent(
    Guid OrderId,
    Guid UserId,
    Guid GameId,
    DateTimeOffset OccurredAtUtc);
