namespace KSE.GameStore.DataAccess.Entities;

public class UserGameStock
{
    public Guid UserId { get; set; }
    public int GameId { get; set; }
    public Guid License { get; set; }

    public required User User { get; set; }
    public required Game Game { get; set; }
}