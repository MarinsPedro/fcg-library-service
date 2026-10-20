namespace FiapCloudGames.Library.Application.Features.Entries.CheckOwnership;

public sealed record CheckGameOwnershipResult(
    Guid UserId,
    Guid GameId,
    bool Owned);
