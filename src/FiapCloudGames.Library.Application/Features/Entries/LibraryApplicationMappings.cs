using FiapCloudGames.Library.Domain.Entries;

namespace FiapCloudGames.Library.Application.Features.Entries;

internal static class LibraryApplicationMappings
{
    public static LibraryEntryResult ToResult(LibraryEntry entry) =>
        new(
            entry.Id,
            entry.UserId,
            entry.GameId,
            entry.OrderId,
            entry.GrantedAtUtc);
}
