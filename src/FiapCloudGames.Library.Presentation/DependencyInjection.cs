using FiapCloudGames.BuildingBlocks.Authentication;
using FiapCloudGames.Library.Presentation.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Library.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddLibraryPresentation(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(DependencyInjection).Assembly);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                LibraryPolicies.GamesRead,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireServiceToken()
                    .RequireScopes("library.games.read"));

            options.AddPolicy(
                LibraryPolicies.OwnershipRead,
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireServiceToken()
                    .RequireScopes("library.ownership.read"));
        });

        return services;
    }
}
