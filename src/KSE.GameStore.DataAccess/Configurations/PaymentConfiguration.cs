using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSE.GameStore.DataAccess.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id");
        
        builder.Property(p => p.OrderId)
            .HasColumnName("order_id");
        
        builder.Property(p => p.PaymentMethod)
            .HasColumnName("payment_method")
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(p => p.Confirmed)
            .HasColumnName("confirmed")
            .IsRequired();
        
        builder.Property(p => p.PayedAt)
            .HasColumnName("payed_at")
            .IsRequired();

        builder.HasOne(p => p.Order)
            .WithOne(o => o.Payment)
            .HasForeignKey<Payment>(p => p.OrderId);
    }
}