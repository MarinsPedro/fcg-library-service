using MassTransit;
using FiapCloudGames.Library.Contracts.Events.V1;

namespace FiapCloudGames.Library.Worker.Messaging;

internal static class RabbitMqTopologyConfigurator
{
    internal static void Configure(
        IRabbitMqBusFactoryConfigurator rabbitMq,
        RabbitMqExchangeOptions exchanges)
    {
        rabbitMq.Message<GameGrantRequestedEventV1>(configuration =>
            configuration.SetEntityName(exchanges.GameGrantRequested));
        rabbitMq.Message<GameGrantedEventV1>(configuration =>
            configuration.SetEntityName(exchanges.GameGranted));
    }
}
