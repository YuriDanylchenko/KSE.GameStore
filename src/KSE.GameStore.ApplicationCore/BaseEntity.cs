using System.ComponentModel.DataAnnotations.Schema;

namespace KSE.GameStore.ApplicationCore;

public class BaseEntity<TKey>
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TKey Id { get; set; } = default!;
}