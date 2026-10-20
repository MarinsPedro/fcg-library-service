using FiapCloudGames.Library.Domain.Common;
using FiapCloudGames.Library.Domain.Entries;
using FiapCloudGames.Library.UnitTests.Support;

namespace FiapCloudGames.Library.UnitTests.Domain.Entries;

public sealed class LibraryEntryTests
{
    [Fact(DisplayName = "Criar entrada - deve criar ownership com dados validos")]
    public void Create_WithValidData_ShouldCreateEntry()
    {
        var entry = LibraryEntry.Create(
            LibraryTestData.UserId,
            LibraryTestData.GameId,
            LibraryTestData.OrderId,
            LibraryTestData.NowUtc);

        Assert.NotEqual(Guid.Empty, entry.Id);
        Assert.Equal(LibraryTestData.UserId, entry.UserId);
        Assert.Equal(LibraryTestData.GameId, entry.GameId);
        Assert.Equal(LibraryTestData.OrderId, entry.OrderId);
        Assert.Equal(LibraryTestData.NowUtc, entry.GrantedAtUtc);
    }

    [Fact(DisplayName = "Criar entrada - deve rejeitar usuario vazio")]
    public void Create_WithEmptyUser_ShouldThrow()
    {
        Assert.Throws<DomainRuleViolationException>(() =>
            LibraryEntry.Create(
                Guid.Empty,
                LibraryTestData.GameId,
                LibraryTestData.OrderId,
                LibraryTestData.NowUtc));
    }

    [Fact(DisplayName = "Criar entrada - deve exigir data UTC")]
    public void Create_WithNonUtcDate_ShouldThrow()
    {
        var nonUtc = new DateTimeOffset(2026, 10, 19, 12, 0, 0, TimeSpan.FromHours(-3));

        Assert.Throws<DomainRuleViolationException>(() =>
            LibraryEntry.Create(
                LibraryTestData.UserId,
                LibraryTestData.GameId,
                LibraryTestData.OrderId,
                nonUtc));
    }
}
