namespace KSE.GameStore.ApplicationCore.Mapping.Games;

public record CreateGameRequest(
    string Title,
    string? Description,
    int PublisherId,
    IReadOnlyList<int> GenreIds,
    IReadOnlyList<int> PlatformIds,
    CreateGamePriceRequest Price,
    IReadOnlyList<int>? RegionPermissionIds
);

public record CreateGamePriceRequest(
    decimal Value,
    int? Stock
);