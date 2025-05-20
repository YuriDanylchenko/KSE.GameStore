namespace KSE.GameStore.DataAccess.Entities;

public class Publisher
{
    public int Id { get; set; }
    public required string Name { get; set; } = null!;
    public string? WebsiteUrl { get; set; }
    public string? Description { get; set; }
    
    public ICollection<Game>? Games { get; set; }
}