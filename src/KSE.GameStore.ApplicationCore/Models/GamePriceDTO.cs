namespace KSE.GameStore.ApplicationCore.Models;

public record GamePriceDTO
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public int? Stock { get; set; }
}