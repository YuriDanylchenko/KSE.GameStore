namespace KSE.GameStore.DataAccess.Entities;

public class Region
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    public ICollection<Game>? Games { get; set; }
}