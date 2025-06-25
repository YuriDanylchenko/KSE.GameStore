namespace KSE.GameStore.ApplicationCore.Models.Output;

public record CartItemDto(
    int Id,
    int    GameId,
    string Title,
    decimal Price,
    int    Quantity);