using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSE.GameStore.DataAccess.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("games");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("id");

        builder.Property(g => g.Title)
            .HasColumnName("title")
            .IsRequired();

        builder.Property(g => g.Description)
            .HasColumnName("description");

        builder.Property(g => g.PublisherId)
            .HasColumnName("publisher_id");

        builder.Property(g => g.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(g => g.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasOne(g => g.Publisher)
            .WithMany(p => p.Games)
            .HasForeignKey(g => g.PublisherId);

        // Table "game_genres" configuration
        builder.HasMany(g => g.Genres)
            .WithMany(ge => ge.Games)
            .UsingEntity<Dictionary<string, object>>(
                "game_genres",
                j => j.HasOne<Genre>().WithMany().HasForeignKey("genre_id"),
                j => j.HasOne<Game>().WithMany().HasForeignKey("game_id"),
                j =>
                {
                    j.HasKey("game_id", "genre_id");
                    j.ToTable("game_genres");
                }
            );

        // Table "game_platforms" configuration
        builder.HasMany(g => g.Platforms)
            .WithMany(p => p.Games)
            .UsingEntity<Dictionary<string, object>>(
                "game_platforms",
                j => j.HasOne<Platform>().WithMany().HasForeignKey("platform_id"),
                j => j.HasOne<Game>().WithMany().HasForeignKey("game_id"),
                j =>
                {
                    j.HasKey("game_id", "platform_id");
                    j.ToTable("game_platforms");
                }
            );

        // Table "game_region_permissions" configurations
        builder.HasMany(g => g.RegionPermissions)
            .WithMany(r => r.Games)
            .UsingEntity<Dictionary<string, object>>(
                "game_region_permissions",
                j => j.HasOne<Region>().WithMany().HasForeignKey("region_id"),
                j => j.HasOne<Game>().WithMany().HasForeignKey("game_id"),
                j =>
                {
                    j.HasKey("game_id", "region_id");
                    j.ToTable("game_region_permissions");
                }
            );
    }
}