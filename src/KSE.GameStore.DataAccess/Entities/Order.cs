namespace KSE.GameStore.DataAccess.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Payment? Payment { get; set; }
    public OrderStatus Status { get; set; }
    public required User User { get; set; }
    public required ICollection<OrderItem> OrderItems { get; set; }
    
}