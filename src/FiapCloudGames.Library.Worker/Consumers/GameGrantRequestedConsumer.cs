
using FiapCloudGames.Library.Application.Features.Entries.Grant;
using FiapCloudGames.Library.Contracts.Events.V1;
using MassTransit;

namespace FiapCloudGames.Library.Worker.Consumers;

public sealed class GameGrantRequestedConsumer(
    GrantGameService grantGameService,
    ILogger<GameGrantRequestedConsumer> logger)
    : IConsumer<GameGrantRequestedEventV1>
{
    public async Task Consume(
        ConsumeContext<GameGrantRequestedEventV1> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Processando GameGrantRequestedEventV1. OrderId={OrderId}, MessageId={MessageId}, CorrelationId={CorrelationId}.",
            message.OrderId,
            context.MessageId,
            context.CorrelationId);

        // Concede o jogo na biblioteca.
       var result = await grantGameService.ExecuteAsync(
        new GrantGameInput(
            message.OrderId,
            message.UserId,
            message.GameId,
            DateTimeOffset.UtcNow),
        context.CancellationToken);

        // Publica a confirmação de concessão.
        
        await context.Publish(
            new GameGrantedEventV1(
                result.Id,
                result.OrderId,
                result.UserId,
                result.GameId,
                result.GrantedAtUtc),
            publishContext =>
            {
                if (context.CorrelationId is Guid correlationId)
                {
                    publishContext.CorrelationId = correlationId;
                }
            },
            context.CancellationToken);


        logger.LogInformation(
            "GameGrantedEventV1 registrado para publicação. LibraryEntryId={LibraryEntryId}, OrderId={OrderId}.",
            result.Id,
            result.OrderId);
    }
}
