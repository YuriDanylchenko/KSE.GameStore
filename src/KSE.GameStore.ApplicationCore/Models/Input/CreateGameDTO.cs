namespace KSE.GameStore.ApplicationCore.Models.Input;

public record CreateGameDTO(
    string Title,
    string? Description,
    int PublisherId,
    IReadOnlyList<int> GenreIds,
    IReadOnlyList<int> PlatformIds,
    CreateGamePriceDTO PriceDto,
    IReadOnlyList<int>? RegionPermissionIds
);

public record CreateGamePriceDTO(
    decimal Value,
    int? Stock
);