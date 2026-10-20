using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.BuildingBlocks.Hosting;
using FiapCloudGames.Library.Infrastructure;
using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Library.Worker.Consumers;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.AddFiapServiceDefaults();
builder.Services.AddLibraryInfrastructure(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCompletedConsumer>();

    x.AddEntityFrameworkOutbox<LibraryDbContext>(outbox =>
    {
        outbox.UsePostgres();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var rabbitPort = ushort.TryParse(
            builder.Configuration["RabbitMq:Port"],
            out var port)
            ? port
            : (ushort)5672;
        var rabbitUsername = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var rabbitPassword = builder.Configuration["RabbitMq:Password"] ?? "guest";

        cfg.Host(
            rabbitHost,
            rabbitPort,
            "/",
            host =>
            {
                host.Username(rabbitUsername);
                host.Password(rabbitPassword);
            });

        cfg.ReceiveEndpoint("fcg.library.order-completed", endpoint =>
        {
            endpoint.PrefetchCount = 1;
            endpoint.ConcurrentMessageLimit = 1;

            endpoint.UseMessageRetry(retry =>
            {
                retry.Ignore<AppException>();
                retry.Intervals(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5));
            });

            endpoint.UseEntityFrameworkOutbox<LibraryDbContext>(context);
            endpoint.ConfigureConsumer<OrderCompletedConsumer>(context);
        });
    });
});

var host = builder.Build();
await host.RunAsync();
