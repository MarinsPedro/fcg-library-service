namespace FiapCloudGames.Library.Worker.Messaging;

internal sealed class RabbitMqOptions
{
    internal const string SectionName = "RabbitMq";

    public string Host { get; init; } = string.Empty;
    public ushort Port { get; init; } = 5672;
    public string VirtualHost { get; init; } = "/";
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public RabbitMqTopologyOptions Topology { get; init; } = new();
}

internal sealed class RabbitMqTopologyOptions
{
    public RabbitMqExchangeOptions Exchanges { get; init; } = new();
    public RabbitMqQueueOptions Queues { get; init; } = new();
}

internal sealed class RabbitMqExchangeOptions
{
    public string GameGrantRequested { get; init; } = string.Empty;
    public string GameGranted { get; init; } = string.Empty;
}

internal sealed class RabbitMqQueueOptions
{
    public string GameGrantRequested { get; init; } = string.Empty;
}
