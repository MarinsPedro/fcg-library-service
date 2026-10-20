using FiapCloudGames.Library.Domain.Entries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Library.Infrastructure.Persistence.Configurations;

internal sealed class LibraryEntryConfiguration : IEntityTypeConfiguration<LibraryEntry>
{
    public void Configure(EntityTypeBuilder<LibraryEntry> builder)
    {
        builder.ToTable("library_entries");
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(entry => entry.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(entry => entry.GameId)
            .HasColumnName("game_id")
            .IsRequired();

        builder.Property(entry => entry.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(entry => entry.GrantedAtUtc)
            .HasColumnName("granted_at_utc")
            .IsRequired();

        builder.HasIndex(entry => new { entry.UserId, entry.GameId })
            .IsUnique()
            .HasDatabaseName("ux_library_entries_user_game");

        builder.HasIndex(entry => entry.OrderId)
            .IsUnique()
            .HasDatabaseName("ux_library_entries_order_id");

        builder.HasIndex(entry => new { entry.UserId, entry.GrantedAtUtc, entry.Id })
            .HasDatabaseName("ix_library_entries_user_granted");
    }
}
