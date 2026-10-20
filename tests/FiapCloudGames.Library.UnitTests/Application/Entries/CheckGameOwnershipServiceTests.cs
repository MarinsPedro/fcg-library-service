using FiapCloudGames.Library.Application.Features.Entries.CheckOwnership;
using FiapCloudGames.Library.Domain.Entries;
using FiapCloudGames.Library.UnitTests.Support;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Library.UnitTests.Application.Entries;

public sealed class CheckGameOwnershipServiceTests
{
    [Fact(DisplayName = "Verificar ownership - deve retornar true quando usuario possui o jogo")]
    public async Task Execute_WhenOwned_ShouldReturnTrue()
    {
        var repository = new FakeLibraryEntryRepository();
        repository.Entries.Add(LibraryEntry.Create(
            LibraryTestData.UserId,
            LibraryTestData.GameId,
            LibraryTestData.OrderId,
            LibraryTestData.NowUtc));

        var service = new CheckGameOwnershipService(
            repository,
            NullLogger<CheckGameOwnershipService>.Instance);

        var result = await service.ExecuteAsync(
            LibraryTestData.UserId,
            LibraryTestData.GameId,
            CancellationToken.None);

        Assert.True(result.Owned);
    }

    [Fact(DisplayName = "Verificar ownership - deve retornar false quando usuario nao possui o jogo")]
    public async Task Execute_WhenNotOwned_ShouldReturnFalse()
    {
        var service = new CheckGameOwnershipService(
            new FakeLibraryEntryRepository(),
            NullLogger<CheckGameOwnershipService>.Instance);

        var result = await service.ExecuteAsync(
            LibraryTestData.UserId,
            LibraryTestData.GameId,
            CancellationToken.None);

        Assert.False(result.Owned);
    }
}
