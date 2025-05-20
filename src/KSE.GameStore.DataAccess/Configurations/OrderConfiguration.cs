using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSE.GameStore.DataAccess.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(o => o.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.Property(o => o.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(o => o.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId);
    }
}