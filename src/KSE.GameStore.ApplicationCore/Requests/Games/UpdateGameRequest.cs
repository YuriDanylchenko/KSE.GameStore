namespace KSE.GameStore.ApplicationCore.Requests.Games;

public record UpdateGameRequest(
    int Id,
    string Title,
    string? Description,
    int PublisherId,
    IReadOnlyList<int> GenreIds,
    IReadOnlyList<int> PlatformIds,
    UpdateGamePriceRequest Price,
    IReadOnlyList<int>? RegionPermissionIds
);

public record UpdateGamePriceRequest(
    decimal Value,
    int? Stock
);