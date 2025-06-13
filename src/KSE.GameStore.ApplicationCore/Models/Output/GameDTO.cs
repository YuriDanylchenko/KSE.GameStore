namespace KSE.GameStore.ApplicationCore.Models.Output;

public record GameDTO(
    int Id,
    string Title,
    string? Description,
    PublisherDTO Publisher,
    IReadOnlyList<GenreDTO> Genres,
    IReadOnlyList<PlatformDTO> Platforms,
    GamePriceDTO Price,
    IReadOnlyList<RegionDTO>? RegionPermissions
);