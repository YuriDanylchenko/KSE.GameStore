namespace KSE.GameStore.ApplicationCore.Models.Publisher;

public class CreatePublisherDTO
{
    public string Name { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public string? Description { get; set; }
}