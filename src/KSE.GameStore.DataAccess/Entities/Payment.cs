using KSE.GameStore.ApplicationCore;

namespace KSE.GameStore.DataAccess.Entities;

public class Payment : BaseEntity<int>
{
    public int OrderId { get; set; }
    public bool Confirmed { get; set; }
    public DateTime PayedAt { get; set; }
    
    public required Order Order { get; set; }
    public required PaymentMethod PaymentMethod { get; set; }
}