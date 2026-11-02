using System.Text;
using System.Text.Json;
using FiapCloudGames.BuildingBlocks.Exceptions;

namespace FiapCloudGames.Library.Application.Features.Entries.List;

internal static class LibraryCursorCodec
{
    private sealed record CursorData(DateTimeOffset GrantedAtUtc, Guid Id);

    public static string Encode(DateTimeOffset grantedAtUtc, Guid id)
    {
        var json = JsonSerializer.Serialize(new CursorData(grantedAtUtc, id));
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    public static (DateTimeOffset GrantedAtUtc, Guid Id)? Decode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return null;

        try
        {
            var normalized = cursor.Replace('-', '+').Replace('_', '/');
            normalized = normalized.PadRight(
                normalized.Length + ((4 - normalized.Length % 4) % 4),
                '=');

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(normalized));
            var data = JsonSerializer.Deserialize<CursorData>(json)
                ?? throw new FormatException();

            return (data.GrantedAtUtc, data.Id);
        }
        catch (Exception exception) when (
            exception is FormatException or JsonException)
        {
            throw AppException.Validation("Cursor de paginação inválido.");
        }
    }
}
