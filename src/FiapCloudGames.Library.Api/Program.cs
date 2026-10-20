using FiapCloudGames.BuildingBlocks.Authentication;
using FiapCloudGames.BuildingBlocks.Hosting;
using FiapCloudGames.BuildingBlocks.Web;
using FiapCloudGames.Library.Api.Configuration;
using FiapCloudGames.Library.Infrastructure;
using FiapCloudGames.Library.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.AddFiapServiceDefaults();
builder.Services.AddFiapApiDefaults();
builder.Services.AddFiapServiceAuthentication(builder.Configuration);
builder.Services.AddLibraryInfrastructure(builder.Configuration);
builder.Services.AddLibraryPresentation();
builder.Services.AddLibraryOpenApi();

var app = builder.Build();

app.UseFiapApiDefaults();
app.UseLibraryOpenApi();
app.UseAuthentication();
app.UseAuthorization();

app.MapFiapHealthEndpoints();
app.MapControllers();

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
