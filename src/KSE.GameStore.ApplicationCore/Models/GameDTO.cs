namespace KSE.GameStore.ApplicationCore.Models;

public record GameDTO(
    int Id,
    string Title,
    string? Description,
    string Publisher,
    IReadOnlyList<GenreDTO> Genres,
    IReadOnlyList<PlatformDTO> Platforms,
    GamePriceDTO Price,
    IReadOnlyList<RegionDTO> RegionPermissions
);
