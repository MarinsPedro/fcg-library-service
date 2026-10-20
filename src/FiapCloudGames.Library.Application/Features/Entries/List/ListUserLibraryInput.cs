namespace FiapCloudGames.Library.Application.Features.Entries.List;

public sealed record ListUserLibraryInput(
    Guid UserId,
    int Limit = 20,
    string? Cursor = null);
