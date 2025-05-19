namespace KSE.GameStore.DataAccess.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<Game>? Games { get; set; }
}