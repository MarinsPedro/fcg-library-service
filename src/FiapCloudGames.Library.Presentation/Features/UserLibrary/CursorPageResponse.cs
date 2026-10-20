namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

public sealed class CursorPageResponse<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public string? NextCursor { get; init; }
    public bool HasMore { get; init; }
}
