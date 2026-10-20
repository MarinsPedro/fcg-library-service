using FiapCloudGames.Library.Domain.Entries;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FiapCloudGames.Library.Database.IntegrationTests;

public sealed class EfModelConventionTests
{
    [Fact(DisplayName = "Verificar LibraryEntry - deve usar schema library e lower_snake_case")]
    public void Check_LibraryEntry_ShouldUseLibrarySchemaAndLowerSnakeCase()
    {
        using var context = LibraryDbContextTestFactory.Create();
        var entity = context.Model.FindEntityType(typeof(LibraryEntry));

        Assert.NotNull(entity);
        Assert.Equal("library", entity.GetSchema());
        Assert.Equal("library_entries", entity.GetTableName());

        var storeObject = StoreObjectIdentifier.Table(
            entity.GetTableName()!,
            entity.GetSchema());

        foreach (var property in entity.GetProperties())
        {
            var column = Assert.IsType<string>(property.GetColumnName(storeObject));
            AssertLowerSnakeCase(column);
        }
    }

    [Fact(DisplayName = "Verificar LibraryEntry - deve proteger ownership e pedido com indices unicos")]
    public void Check_LibraryEntry_ShouldHaveOwnershipAndOrderUniqueIndexes()
    {
        using var context = LibraryDbContextTestFactory.Create();
        var entity = context.Model.FindEntityType(typeof(LibraryEntry));
        Assert.NotNull(entity);

        var indexes = entity.GetIndexes().ToArray();

        Assert.Contains(indexes, index =>
            index.IsUnique &&
            index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(LibraryEntry.UserId), nameof(LibraryEntry.GameId)]));

        Assert.Contains(indexes, index =>
            index.IsUnique &&
            index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(LibraryEntry.OrderId)]));
    }

    [Fact(DisplayName = "Verificar MassTransit - deve mapear inbox e outbox no schema library")]
    public void Check_MassTransitOutbox_ShouldUseLibrarySchema()
    {
        using var context = LibraryDbContextTestFactory.Create();
        var tables = context.Model.GetEntityTypes()
            .Select(entity => (entity.GetTableName(), entity.GetSchema()))
            .ToArray();

        Assert.Contains(("inbox_state", "library"), tables);
        Assert.Contains(("outbox_message", "library"), tables);
        Assert.Contains(("outbox_state", "library"), tables);
    }

    private static void AssertLowerSnakeCase(string value)
    {
        Assert.True(
            value.Length > 0 &&
            char.IsLower(value[0]) &&
            value.All(character =>
                char.IsLower(character) ||
                char.IsDigit(character) ||
                character == '_'),
            $"'{value}' nao segue lower_snake_case.");
    }
}
