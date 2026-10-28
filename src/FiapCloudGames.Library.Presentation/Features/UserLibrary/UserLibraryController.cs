using FiapCloudGames.BuildingBlocks.Web;
using FiapCloudGames.Library.Application.Features.Entries.Get;
using FiapCloudGames.Library.Application.Features.Entries.List;
using FiapCloudGames.Library.Presentation.Authorization;
using FiapCloudGames.Library.Presentation.Features.Entries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Library.Presentation.Features.UserLibrary;

[ApiController]
[Route("v1/library/users/{userId:guid}/games")]
public sealed class UserLibraryController : ControllerBase
{
    [Authorize(Policy = LibraryPolicies.GamesRead)]
    [HttpGet]
    [ProducesResponseType(typeof(CursorPageResponse<LibraryEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CursorPageResponse<LibraryEntryResponse>>> ListAsync(
        [FromRoute] Guid userId,
        [FromQuery] UserLibraryQueryRequest request,
        [FromServices] ListUserLibraryService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            new ListUserLibraryInput(userId, request.Limit, request.Cursor),
            cancellationToken);

        return Ok(new CursorPageResponse<LibraryEntryResponse>
        {
            Items = result.Items.Select(item => item.ToResponse()).ToArray(),
            NextCursor = result.NextCursor,
            HasMore = result.HasMore
        });
    }

    [Authorize(Policy = LibraryPolicies.GamesRead)]
    [HttpGet("entries/{entryId:guid}")]
    [ProducesResponseType(typeof(LibraryEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LibraryEntryResponse>> GetEntryAsync(
        [FromRoute] Guid userId,
        [FromRoute] Guid entryId,
        [FromServices] GetLibraryEntryService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(entryId, userId, cancellationToken);
        return Ok(result.ToResponse());
    }
}
