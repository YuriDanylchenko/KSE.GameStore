using KSE.GameStore.ApplicationCore;

namespace KSE.GameStore.DataAccess.Entities;

public class Game : BaseEntity<int>
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int PublisherId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public required Publisher Publisher { get; set; }
    public required ICollection<Genre> Genres { get; set; }
    public required ICollection<Platform> Platforms { get; set; }
    public required ICollection<GamePrice> Prices { get; set; }
    public ICollection<Region>? RegionPermissions { get; set; }
}