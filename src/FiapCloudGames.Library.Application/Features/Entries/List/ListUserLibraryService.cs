using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Library.Domain.Entries;
using FiapCloudGames.Library.Domain.Entries.Queries;
using Microsoft.Extensions.Logging;

namespace FiapCloudGames.Library.Application.Features.Entries.List;

public sealed class ListUserLibraryService(
    ILibraryEntryRepository repository,
    ILogger<ListUserLibraryService> logger)
{
    public async Task<CursorPageResult<LibraryEntryResult>> ExecuteAsync(
        ListUserLibraryInput input,
        CancellationToken cancellationToken)
    {
        if (input.Limit is < 1 or > 100)
            throw AppException.Validation("O limite deve estar entre 1 e 100.");

        var cursor = LibraryCursorCodec.Decode(input.Cursor);

        logger.LogInformation(
            "Listando biblioteca do usuario {UserId} com limite {Limit}.",
            input.UserId,
            input.Limit);

        var records = await repository.ListAsync(
            new LibraryEntryQueryParameters
            {
                UserId = input.UserId,
                BeforeGrantedAtUtc = cursor?.GrantedAtUtc,
                BeforeId = cursor?.Id,
                Limit = input.Limit + 1
            },
            cancellationToken);

        var hasMore = records.Count > input.Limit;
        var page = records.Take(input.Limit).ToArray();
        var last = page.LastOrDefault();

        return new CursorPageResult<LibraryEntryResult>
        {
            Items = page.Select(LibraryApplicationMappings.ToResult).ToArray(),
            HasMore = hasMore,
            NextCursor = hasMore && last is not null
                ? LibraryCursorCodec.Encode(last.GrantedAtUtc, last.Id)
                : null
        };
    }
}
