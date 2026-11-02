using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Library.Infrastructure.Persistence;
using FiapCloudGames.Library.Worker.Consumers;
using FiapCloudGames.Library.Worker.Messaging;
using MassTransit;
using Microsoft.Extensions.Options;

namespace FiapCloudGames.Library.Worker;

internal static class DependencyInjection
{
    internal static IServiceCollection AddLibraryWorker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddRabbitMqOptions(services, configuration);
        AddMassTransit(services);
        return services;
    }

    private static void AddRabbitMqOptions(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Host),
                "RabbitMq:Host é obrigatório.")
            .Validate(
                options => options.Port > 0,
                "RabbitMq:Port deve ser maior que zero.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.VirtualHost),
                "RabbitMq:VirtualHost é obrigatório.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Username),
                "RabbitMq:Username é obrigatório.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Password),
                "RabbitMq:Password é obrigatório.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(
                        options.Topology.Exchanges.GameGrantRequested) &&
                    !string.IsNullOrWhiteSpace(
                        options.Topology.Exchanges.GameGranted) &&
                    !string.IsNullOrWhiteSpace(
                        options.Topology.Queues.GameGrantRequested),
                "Todos os nomes em RabbitMq:Topology são obrigatórios.")
            .ValidateOnStart();
    }

    private static void AddMassTransit(IServiceCollection services)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<GameGrantRequestedConsumer>();
            configurator.AddEntityFrameworkOutbox<LibraryDbContext>(outbox =>
            {
                outbox.UsePostgres();
            });

            configurator.UsingRabbitMq((context, rabbitMq) =>
            {
                var options = context
                    .GetRequiredService<IOptions<RabbitMqOptions>>()
                    .Value;

                RabbitMqTopologyConfigurator.Configure(
                    rabbitMq,
                    options.Topology.Exchanges);

                rabbitMq.Host(
                    options.Host,
                    options.Port,
                    options.VirtualHost,
                    host =>
                    {
                        host.Username(options.Username);
                        host.Password(options.Password);
                    });

                rabbitMq.ReceiveEndpoint(
                    options.Topology.Queues.GameGrantRequested,
                    endpoint =>
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
                        endpoint.ConfigureConsumer<GameGrantRequestedConsumer>(context);
                    });
            });
        });
    }
}
