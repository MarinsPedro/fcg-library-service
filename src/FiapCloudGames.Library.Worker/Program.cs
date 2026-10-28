using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.BuildingBlocks.Hosting;
using FiapCloudGames.Library.Infrastructure;
using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Library.Worker.Consumers;
using MassTransit;
using FiapCloudGames.Library.Contracts.Events.V1;

var builder = Host.CreateApplicationBuilder(args);

builder.AddFiapServiceDefaults();
builder.Services.AddLibraryInfrastructure(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GameGrantRequestedConsumer>();  

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

        
            var libraryExchange = "fcg.library.events";

            // Disponibiliza o serializador JSON sem envelope.
            cfg.AddRawJsonSerializer();

            // Define a exchange de publicação.
            cfg.Message<GameGrantedEventV1>(message =>
            {
                message.SetEntityName(libraryExchange);
            });

            // Define a routing key e o formato JSON.
            cfg.Send<GameGrantedEventV1>(send =>
            {
                send.UseRoutingKeyFormatter(
                    _ => "library.game.granted");

                send.UseSerializer("application/json");
            });

            // Configura a exchange como topic.
            cfg.Publish<GameGrantedEventV1>(publish =>
            {
                publish.ExchangeType = "topic";
            });
        
            
        cfg.ReceiveEndpoint("fcg.library.game-grant-requested", endpoint =>
        {
            
            endpoint.ConfigureConsumeTopology = false;

            // Permite consumir JSON sem envelope MassTransit.
            endpoint.UseRawJsonDeserializer(
                RawSerializerOptions.All,
                isDefault: true);

            // Vincula a fila à exchange de entrada.
            endpoint.Bind("fcg.events", binding =>
            {
                binding.ExchangeType = "topic";
                binding.RoutingKey = "library.game.grant.requested";
            });

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

            endpoint.ConfigureConsumer<GameGrantRequestedConsumer>(context);
        });


    });
});

var host = builder.Build();
await host.RunAsync();
