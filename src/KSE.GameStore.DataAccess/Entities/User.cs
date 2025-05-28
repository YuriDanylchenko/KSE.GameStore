namespace KSE.GameStore.DataAccess.Entities;

public class User : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public int RegionId { get; set; }
    
    public required Region Region { get; set; }
    public ICollection<Order>? Orders { get; set; }
}