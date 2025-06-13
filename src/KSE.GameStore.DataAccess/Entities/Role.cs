namespace KSE.GameStore.DataAccess.Entities;

public class Role : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}