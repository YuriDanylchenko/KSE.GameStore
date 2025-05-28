namespace KSE.GameStore.DataAccess.Entities;

public class OrderItem : BaseEntity<int>
{
    public int OrderId { get; set; }
    public int GameId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public required Order Order { get; set; }
    public required Game Game { get; set; }
    
}