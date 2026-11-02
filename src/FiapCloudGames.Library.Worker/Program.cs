using FiapCloudGames.BuildingBlocks.Hosting;
using FiapCloudGames.Library.Infrastructure;
using FiapCloudGames.Library.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddFiapServiceDefaults();
builder.Services.AddLibraryInfrastructure(builder.Configuration);
builder.Services.AddLibraryWorker(builder.Configuration);

var host = builder.Build();
await host.RunAsync();
