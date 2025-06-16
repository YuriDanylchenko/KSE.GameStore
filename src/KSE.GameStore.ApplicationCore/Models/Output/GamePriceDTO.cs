namespace KSE.GameStore.ApplicationCore.Models.Output;

public record GamePriceDTO(
    int Id,
    decimal Value,
    int? Stock
);