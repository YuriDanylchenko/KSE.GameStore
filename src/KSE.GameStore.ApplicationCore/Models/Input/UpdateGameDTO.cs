namespace KSE.GameStore.ApplicationCore.Models.Input;

public record UpdateGameDTO(
    int Id,
    string Title,
    string? Description,
    int PublisherId,
    IReadOnlyList<int> GenreIds,
    IReadOnlyList<int> PlatformIds,
    UpdateGamePriceDTO PriceDto,
    IReadOnlyList<int>? RegionPermissionIds
);

public record UpdateGamePriceDTO(
    decimal Value,
    int? Stock
);