namespace KSE.GameStore.DataAccess.Entities;

public class GamePrice : BaseEntity<int>
{
    public int GameId { get; set; }
    public decimal Value { get; set; }
    public int Stock { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public required Game Game { get; set; }
}