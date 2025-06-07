namespace KSE.GameStore.ApplicationCore.Models;

public class GameDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PublisherDTO Publisher { get; set; } = null!;
    public IReadOnlyList<GenreDTO> Genres { get; set; } = new List<GenreDTO>();
    public IReadOnlyList<PlatformDTO> Platforms { get; set; } = new List<PlatformDTO>();
    public GamePriceDTO Price { get; set; } = null!;
    public IReadOnlyList<RegionDTO>? RegionPermissions { get; set; }
}