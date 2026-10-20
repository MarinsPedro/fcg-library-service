namespace FiapCloudGames.Library.Application.Features.Entries.List;

public sealed class CursorPageResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public string? NextCursor { get; init; }
    public bool HasMore { get; init; }
}
