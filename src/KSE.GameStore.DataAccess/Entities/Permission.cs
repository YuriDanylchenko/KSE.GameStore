namespace KSE.GameStore.DataAccess.Entities;

public class Permission : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}