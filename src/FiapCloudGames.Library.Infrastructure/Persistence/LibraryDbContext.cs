using FiapCloudGames.Library.Application.Abstractions.Persistence;
using FiapCloudGames.Library.Domain.Entries;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Library.Infrastructure.Persistence;

public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options)
    : DbContext(options), ILibraryUnitOfWork
{
    public DbSet<LibraryEntry> LibraryEntries => Set<LibraryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("library");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);

        modelBuilder.AddInboxStateEntity(entity => entity.ToTable("inbox_state"));
        modelBuilder.AddOutboxMessageEntity(entity => entity.ToTable("outbox_message"));
        modelBuilder.AddOutboxStateEntity(entity => entity.ToTable("outbox_state"));
    }
}
