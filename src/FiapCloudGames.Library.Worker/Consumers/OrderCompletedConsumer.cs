using FiapCloudGames.Library.Application.Features.Entries.Grant;
using FiapCloudGames.Library.Contracts.Events;
using MassTransit;

namespace FiapCloudGames.Library.Worker.Consumers;

public sealed class OrderCompletedConsumer(
    GrantGameService grantGameService,
    ILogger<OrderCompletedConsumer> logger)
    : IConsumer<OrderCompletedEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Processando OrderCompletedEvent. OrderId={OrderId}, MessageId={MessageId}, CorrelationId={CorrelationId}.",
            message.OrderId,
            context.MessageId,
            context.CorrelationId);

        var result = await grantGameService.ExecuteAsync(
            new GrantGameInput(
                message.OrderId,
                message.UserId,
                message.GameId,
                message.OccurredAtUtc),
            context.CancellationToken);

        logger.LogInformation(
            "OrderCompletedEvent processado. LibraryEntryId={LibraryEntryId}.",
            result.Id);
    }
}
