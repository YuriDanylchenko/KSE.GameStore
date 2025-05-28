namespace KSE.GameStore.ApplicationCore.Requests.Genre;

public class UpdateGenreRequest
{
    public required int Id { get; set; }
    public required string Name { get; set; } = null!;
}