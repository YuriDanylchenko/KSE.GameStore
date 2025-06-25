using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;

namespace KSE.GameStore.DataAccess.Configurations;

public class UserGameStockConfiguration : IEntityTypeConfiguration<UserGameStock>
{
    public void Configure(EntityTypeBuilder<UserGameStock> builder)
    {
        builder.ToTable("user_game_stock");
        
        builder.HasKey(ugs => ugs.Id);
        
        builder.Property(ugs => ugs.Id)
            .HasColumnName("id");

        builder.HasKey(ugs => new { ugs.UserId, ugs.GameId });

        builder.Property(ugs => ugs.License)
            .HasColumnName("license")
            .IsRequired();

        builder.Property(ugs => ugs.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(ugs => ugs.GameId)
            .HasColumnName("game_id")
            .IsRequired();

        builder.HasOne(ugs => ugs.User)
            .WithMany(u => u.GameStock)
            .HasForeignKey(ugs => ugs.UserId);

        builder.HasOne(ugs => ugs.Game)
            .WithMany(g => g.Customers)
            .HasForeignKey(ugs => ugs.GameId);
    }
}