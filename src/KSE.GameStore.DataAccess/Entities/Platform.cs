namespace KSE.GameStore.DataAccess.Entities;

public class Platform : BaseEntity<int>
{
    public required string Name { get; set; } = null!;
    
    public ICollection<Game>? Games { get; set; }
}