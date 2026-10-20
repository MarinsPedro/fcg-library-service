using System.Net;
using FiapCloudGames.Library.IntegrationTests.Support;

namespace FiapCloudGames.Library.IntegrationTests.Contracts;

public sealed class LibraryEndpointTests(LibraryApiFactory factory)
    : IClassFixture<LibraryApiFactory>
{
    [Fact]
    public async Task CheckOwnership_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        using var response = await client.GetAsync(
            $"/internal/v1/library/users/{userId}/games/{gameId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListLibrary_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();
        var userId = Guid.NewGuid();

        using var response = await client.GetAsync(
            $"/v1/library/users/{userId}/games");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
