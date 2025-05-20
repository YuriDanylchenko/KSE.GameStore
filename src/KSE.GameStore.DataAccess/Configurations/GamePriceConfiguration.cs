using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSE.GameStore.DataAccess.Configurations;

public class GamePriceConfiguration : IEntityTypeConfiguration<GamePrice>
{
    public void Configure(EntityTypeBuilder<GamePrice> builder)
    {
        builder.ToTable("game_prices");

        builder.HasKey(gp => gp.Id);

        builder.Property(gp => gp.Id)
            .HasColumnName("id");
        
        builder.Property(gp => gp.GameId)
            .HasColumnName("game_id");
        
        builder.Property(gp => gp.Value)
            .HasColumnName("price_value");
        
        builder.Property(gp => gp.Stock)
            .HasColumnName("stock");
        
        builder.Property(gp => gp.StartDate)
            .HasColumnName("start_date");
        
        builder.Property(gp => gp.EndDate)
            .HasColumnName("end_date");

        builder.HasOne(gp => gp.Game)
            .WithMany(g => g.Prices)
            .HasForeignKey(gp => gp.GameId);
    }
}