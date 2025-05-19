namespace KSE.GameStore.DataAccess.Entities;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int PaymentMethodId { get; set; }
    public bool Confirmed { get; set; }
    public DateTime PayedAt { get; set; }
    
    public required Order Order { get; set; }
    public required PaymentMethod PaymentMethod { get; set; }
}