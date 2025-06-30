namespace KSE.GameStore.DataAccess.Entities;

public class OrderItem : BaseEntity<int>
{
    public int OrderId { get; set; }
    public int GameId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public Order Order { get; set; }
    public Game Game { get; set; }
}