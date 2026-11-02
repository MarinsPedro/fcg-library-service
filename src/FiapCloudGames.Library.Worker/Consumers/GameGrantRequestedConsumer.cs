using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Library.Application.Features.Entries.Grant;
using FiapCloudGames.Library.Contracts.Events.V1;
using MassTransit;

namespace FiapCloudGames.Library.Worker.Consumers;

public sealed class GameGrantRequestedConsumer(
    GrantGameService grantGameService,
    TimeProvider clock,
    ILogger<GameGrantRequestedConsumer> logger)
    : IConsumer<GameGrantRequestedEventV1>
{
    public async Task Consume(
        ConsumeContext<GameGrantRequestedEventV1> context)
    {
        var message = context.Message;
        Validate(message);

        logger.LogInformation(
            "Processando GameGrantRequestedEventV1. OrderId={OrderId}, MessageId={MessageId}, CorrelationId={CorrelationId}.",
            message.OrderId,
            context.MessageId,
            context.CorrelationId);

        var result = await grantGameService.ExecuteAsync(
            new GrantGameInput(
                message.OrderId,
                message.UserId,
                message.GameId,
                clock.GetUtcNow()),
            context.CancellationToken);

        await context.Publish(
            new GameGrantedEventV1(
                result.Id,
                result.OrderId,
                result.UserId,
                result.GameId,
                result.GrantedAtUtc),
            publishContext =>
            {
                publishContext.MessageId = publishContext.Message.EventId;

                if (context.CorrelationId is Guid correlationId)
                {
                    publishContext.CorrelationId = correlationId;
                }
                else
                {
                    publishContext.CorrelationId = result.OrderId;
                }
            },
            context.CancellationToken);
        logger.LogInformation(
            "GameGrantedEventV1 registrado para publicação. LibraryEntryId={LibraryEntryId}, OrderId={OrderId}.",
            result.Id,
            result.OrderId);
    }

    private static void Validate(GameGrantRequestedEventV1 message)
    {
        List<AppError> errors = [];

        if (message.EventId == Guid.Empty)
        {
            errors.Add(new AppError(
                "O identificador do evento é obrigatório.",
                "eventId"));
        }

        if (message.OccurredAtUtc == default ||
            message.OccurredAtUtc.Offset != TimeSpan.Zero)
        {
            errors.Add(new AppError(
                "A data de ocorrência do evento deve estar em UTC.",
                "occurredAtUtc"));
        }

        if (errors.Count > 0)
        {
            throw AppException.Validation(errors);
        }
    }
}
