namespace KSE.GameStore.DataAccess.Entities;

public class Region : BaseEntity<int>
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;

    public ICollection<Game>? Games { get; set; }
    public ICollection<User>? Users { get; set; }
}