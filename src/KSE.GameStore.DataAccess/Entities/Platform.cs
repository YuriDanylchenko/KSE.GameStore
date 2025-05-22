namespace KSE.GameStore.DataAccess.Entities;

public class Platform
{
    public int Id { get; set; }
    public required string Name { get; set; } = null!;
    
    public ICollection<Game>? Games { get; set; }
}