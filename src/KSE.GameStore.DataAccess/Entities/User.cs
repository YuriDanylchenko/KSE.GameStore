namespace KSE.GameStore.DataAccess.Entities;

public class User : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string HashedPassword { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public int RegionId { get; set; }

    public required Region Region { get; set; }
    public ICollection<Order>? Orders { get; set; }
    public ICollection<UserGameStock> GameStock { get; set; } = new List<UserGameStock>();

    public ICollection<UserRole> UserRoles { get; set; } = [];
}