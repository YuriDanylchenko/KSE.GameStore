using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSE.GameStore.DataAccess.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id)
            .HasColumnName("id");

        builder.Property(oi => oi.OrderId)
            .HasColumnName("order_id");

        builder.Property(oi => oi.GameId)
            .HasColumnName("game_id");

        builder.Property(oi => oi.Price)
            .HasColumnName("price");

        builder.Property(oi => oi.Quantity)
            .HasColumnName("quantity");

        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        builder.HasOne(oi => oi.Game)
            .WithMany()
            .HasForeignKey(oi => oi.GameId);
    }
}