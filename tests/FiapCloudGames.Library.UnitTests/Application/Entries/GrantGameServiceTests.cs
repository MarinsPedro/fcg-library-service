using FiapCloudGames.BuildingBlocks.Exceptions;
using FiapCloudGames.Library.Application.Features.Entries.Grant;
using FiapCloudGames.Library.Domain.Entries;
using FiapCloudGames.Library.UnitTests.Support;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Library.UnitTests.Application.Entries;

public sealed class GrantGameServiceTests
{
    [Fact(DisplayName = "Conceder jogo - deve persistir uma nova entrada")]
    public async Task Execute_WithNewOwnership_ShouldPersistEntry()
    {
        var repository = new FakeLibraryEntryRepository();
        var unitOfWork = new SpyLibraryUnitOfWork();
        var service = new GrantGameService(
            repository,
            unitOfWork,
            NullLogger<GrantGameService>.Instance);

        var result = await service.ExecuteAsync(
            new GrantGameInput(
                LibraryTestData.OrderId,
                LibraryTestData.UserId,
                LibraryTestData.GameId,
                LibraryTestData.NowUtc),
            CancellationToken.None);

        Assert.NotNull(repository.AddedEntry);
        Assert.Equal(LibraryTestData.UserId, repository.AddedEntry.UserId);
        Assert.Equal(LibraryTestData.GameId, repository.AddedEntry.GameId);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
        Assert.Equal(repository.AddedEntry.Id, result.Id);
    }

    [Fact(DisplayName = "Conceder jogo - deve ser idempotente para o mesmo pedido")]
    public async Task Execute_WithSameOrder_ShouldReturnExistingEntry()
    {
        var repository = new FakeLibraryEntryRepository();
        var existing = LibraryEntry.Create(
            LibraryTestData.UserId,
            LibraryTestData.GameId,
            LibraryTestData.OrderId,
            LibraryTestData.NowUtc);
        repository.Entries.Add(existing);
        var unitOfWork = new SpyLibraryUnitOfWork();
        var service = new GrantGameService(
            repository,
            unitOfWork,
            NullLogger<GrantGameService>.Instance);

        var result = await service.ExecuteAsync(
            new GrantGameInput(
                LibraryTestData.OrderId,
                LibraryTestData.UserId,
                LibraryTestData.GameId,
                LibraryTestData.NowUtc),
            CancellationToken.None);

        Assert.Equal(existing.Id, result.Id);
        Assert.Null(repository.AddedEntry);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact(DisplayName = "Conceder jogo - deve rejeitar ownership existente por outro pedido")]
    public async Task Execute_WithExistingOwnershipFromAnotherOrder_ShouldThrow()
    {
        var repository = new FakeLibraryEntryRepository();
        repository.Entries.Add(LibraryEntry.Create(
            LibraryTestData.UserId,
            LibraryTestData.GameId,
            Guid.NewGuid(),
            LibraryTestData.NowUtc));
        var service = new GrantGameService(
            repository,
            new SpyLibraryUnitOfWork(),
            NullLogger<GrantGameService>.Instance);

        await Assert.ThrowsAsync<AppException>(() =>
            service.ExecuteAsync(
                new GrantGameInput(
                    LibraryTestData.OrderId,
                    LibraryTestData.UserId,
                    LibraryTestData.GameId,
                    LibraryTestData.NowUtc),
                CancellationToken.None));
    }
}
