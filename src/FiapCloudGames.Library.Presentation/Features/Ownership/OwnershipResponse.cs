namespace FiapCloudGames.Library.Presentation.Features.Ownership;

public sealed record OwnershipResponse(
    Guid UserId,
    Guid GameId,
    bool Owned);
