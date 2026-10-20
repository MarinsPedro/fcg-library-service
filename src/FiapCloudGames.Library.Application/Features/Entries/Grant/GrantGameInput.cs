namespace FiapCloudGames.Library.Application.Features.Entries.Grant;

public sealed record GrantGameInput(
    Guid OrderId,
    Guid UserId,
    Guid GameId,
    DateTimeOffset OccurredAtUtc);
