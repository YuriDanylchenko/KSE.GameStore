using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KSE.GameStore.DataAccess.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired();

        builder.HasData(
            new Permission { Id = 1, Name = "All.View" }, // general permission (except Guest)
            new Permission { Id = 2, Name = "All.Create" }, // admin permission
            new Permission { Id = 3, Name = "All.Update" }, // admin permission
            new Permission { Id = 4, Name = "All.Delete" }, // admin permission
            new Permission { Id = 5, Name = "Games.View" }, // only permission for Guest
            new Permission { Id = 6, Name = "Games.Create" }, // manager permission
            new Permission { Id = 7, Name = "Games.Update" }, // manager permission
            new Permission { Id = 8, Name = "Games.Delete" }, // manager permission
            new Permission { Id = 9, Name = "Users.Create" }, // user permission
            new Permission { Id = 10, Name = "Users.Update" }, // user permission
            new Permission { Id = 11, Name = "Users.Delete" } // user permission
        );
    }
}