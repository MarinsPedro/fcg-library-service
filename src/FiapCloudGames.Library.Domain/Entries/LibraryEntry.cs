using FiapCloudGames.Library.Domain.Common;

namespace FiapCloudGames.Library.Domain.Entries;

public sealed class LibraryEntry
{
    public LibraryEntry() { }

    private LibraryEntry(
        Guid id,
        Guid userId,
        Guid gameId,
        Guid orderId,
        DateTimeOffset grantedAtUtc)
    {
        Id = ValidateId(id, "O identificador da entrada da biblioteca e obrigatorio.");
        UserId = ValidateId(userId, "O identificador do usuario e obrigatorio.");
        GameId = ValidateId(gameId, "O identificador do jogo e obrigatorio.");
        OrderId = ValidateId(orderId, "O identificador do pedido e obrigatorio.");
        GrantedAtUtc = ValidateUtc(grantedAtUtc);
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public Guid OrderId { get; private set; }
    public DateTimeOffset GrantedAtUtc { get; private set; }

    public static LibraryEntry Create(
        Guid userId,
        Guid gameId,
        Guid orderId,
        DateTimeOffset grantedAtUtc)
    {
        return new LibraryEntry(
            Guid.NewGuid(),
            userId,
            gameId,
            orderId,
            grantedAtUtc);
    }

    private static Guid ValidateId(Guid id, string message) =>
        id != Guid.Empty
            ? id
            : throw new DomainRuleViolationException(message);

    private static DateTimeOffset ValidateUtc(DateTimeOffset value) =>
        value != default && value.Offset == TimeSpan.Zero
            ? value
            : throw new DomainRuleViolationException(
                "A data de concessao do jogo deve estar em UTC.");
}
