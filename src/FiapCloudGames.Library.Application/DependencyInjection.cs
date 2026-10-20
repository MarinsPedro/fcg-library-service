using FiapCloudGames.Library.Application.Features.Entries.CheckOwnership;
using FiapCloudGames.Library.Application.Features.Entries.Get;
using FiapCloudGames.Library.Application.Features.Entries.Grant;
using FiapCloudGames.Library.Application.Features.Entries.List;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Library.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddLibraryApplication(this IServiceCollection services)
    {
        services.AddScoped<CheckGameOwnershipService>();
        services.AddScoped<GetLibraryEntryService>();
        services.AddScoped<GrantGameService>();
        services.AddScoped<ListUserLibraryService>();

        return services;
    }
}
