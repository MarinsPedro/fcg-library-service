using FiapCloudGames.Library.Application.Features.Entries;

namespace FiapCloudGames.Library.Presentation.Features.Entries;

internal static class LibraryHttpMappings
{
    public static LibraryEntryResponse ToResponse(this LibraryEntryResult result) =>
        new(
            result.Id,
            result.UserId,
            result.GameId,
            result.OrderId,
            result.GrantedAtUtc);
}
