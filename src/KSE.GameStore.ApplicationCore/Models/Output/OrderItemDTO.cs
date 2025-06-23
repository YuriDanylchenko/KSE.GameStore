namespace KSE.GameStore.ApplicationCore.Models.Output;

public record OrderItemDTO(
    int Id,
    int GameId,
    decimal Price,
    int Quantity
);