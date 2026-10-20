using Microsoft.OpenApi;

namespace FiapCloudGames.Library.Api.Configuration;

internal static class LibraryOpenApiExtensions
{
    public static IServiceCollection AddLibraryOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Fiap Cloud Games Library API",
                    Version = "v1",
                    Description = "Servico de biblioteca de jogos dos usuarios."
                });

            options.AddSecurityDefinition(
                "bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Informe somente o access token service, sem o prefixo Bearer."
                });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = []
                });
        });

        return services;
    }

    public static WebApplication UseLibraryOpenApi(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "Fiap Cloud Games Library API v1");
            options.EnablePersistAuthorization();
        });

        return app;
    }
}
