using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

public sealed class UserLibraryQueryRequest
{
    [Range(1, 100)]
    public int Limit { get; init; } = 20;

    public string? Cursor { get; init; }
}
