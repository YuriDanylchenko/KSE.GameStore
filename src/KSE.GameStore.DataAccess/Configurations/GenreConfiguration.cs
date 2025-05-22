using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSE.GameStore.DataAccess.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        // TODO: parent_id for sub genres
        
        builder.ToTable("genres");
        
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("id");
        
        builder.Property(g => g.Name)
            .HasColumnName("name")
            .IsRequired();
    }
}