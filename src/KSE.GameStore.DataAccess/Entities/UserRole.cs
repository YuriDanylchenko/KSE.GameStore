namespace KSE.GameStore.DataAccess.Entities;

public class UserRole : BaseEntity<int>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}