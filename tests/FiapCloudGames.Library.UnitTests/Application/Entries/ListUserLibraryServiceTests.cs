using FiapCloudGames.Library.Application.Features.Entries.List;
using FiapCloudGames.Library.Domain.Entries;
using FiapCloudGames.Library.UnitTests.Support;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiapCloudGames.Library.UnitTests.Application.Entries;

public sealed class ListUserLibraryServiceTests
{
    [Fact(DisplayName = "Listar biblioteca - deve retornar cursor quando houver proxima pagina")]
    public async Task Execute_WithMoreRecords_ShouldReturnNextCursor()
    {
        var repository = new FakeLibraryEntryRepository();
        repository.Entries.Add(LibraryEntry.Create(
            LibraryTestData.UserId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            LibraryTestData.NowUtc));
        repository.Entries.Add(LibraryEntry.Create(
            LibraryTestData.UserId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            LibraryTestData.NowUtc.AddMinutes(-1)));

        var service = new ListUserLibraryService(
            repository,
            NullLogger<ListUserLibraryService>.Instance);

        var result = await service.ExecuteAsync(
            new ListUserLibraryInput(LibraryTestData.UserId, Limit: 1),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.True(result.HasMore);
        Assert.False(string.IsNullOrWhiteSpace(result.NextCursor));
    }

    [Fact(DisplayName = "Listar biblioteca - deve usar cursor para buscar a proxima pagina")]
    public async Task Execute_WithCursor_ShouldReturnNextRecord()
    {
        var repository = new FakeLibraryEntryRepository();
        var newest = LibraryEntry.Create(
            LibraryTestData.UserId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            LibraryTestData.NowUtc);
        var oldest = LibraryEntry.Create(
            LibraryTestData.UserId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            LibraryTestData.NowUtc.AddMinutes(-1));
        repository.Entries.Add(newest);
        repository.Entries.Add(oldest);

        var service = new ListUserLibraryService(
            repository,
            NullLogger<ListUserLibraryService>.Instance);

        var firstPage = await service.ExecuteAsync(
            new ListUserLibraryInput(LibraryTestData.UserId, Limit: 1),
            CancellationToken.None);
        var secondPage = await service.ExecuteAsync(
            new ListUserLibraryInput(
                LibraryTestData.UserId,
                Limit: 1,
                Cursor: firstPage.NextCursor),
            CancellationToken.None);

        Assert.Single(secondPage.Items);
        Assert.Equal(oldest.Id, secondPage.Items[0].Id);
        Assert.False(secondPage.HasMore);
        Assert.Null(secondPage.NextCursor);
    }
}
