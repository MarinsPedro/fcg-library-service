using FiapCloudGames.BuildingBlocks.Web;
using FiapCloudGames.Library.Application.Features.Entries.CheckOwnership;
using FiapCloudGames.Library.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Library.Presentation.Features.Ownership;

[ApiController]
[Route("internal/v1/library/users/{userId:guid}/games")]
public sealed class LibraryOwnershipController : ControllerBase
{
    [Authorize(Policy = LibraryPolicies.OwnershipRead)]
    [HttpGet("{gameId:guid}")]
    [ProducesResponseType(typeof(OwnershipResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OwnershipResponse>> CheckAsync(
        [FromRoute] Guid userId,
        [FromRoute] Guid gameId,
        [FromServices] CheckGameOwnershipService service,
        CancellationToken cancellationToken)
    {
        var result = await service.ExecuteAsync(
            userId,
            gameId,
            cancellationToken);

        return Ok(new OwnershipResponse(
            result.UserId,
            result.GameId,
            result.Owned));
    }
}
