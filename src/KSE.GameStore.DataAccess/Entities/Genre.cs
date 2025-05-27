using KSE.GameStore.ApplicationCore;

namespace KSE.GameStore.DataAccess.Entities;

public class Genre : BaseEntity<int>
{
    public required string Name { get; set; } = null!;
    
    public ICollection<Game>? Games { get; set; }
}