namespace KSE.GameStore.ApplicationCore.Models;

public record GamePriceDTO(
    int Id,
    decimal Value,
    int? Stock
);