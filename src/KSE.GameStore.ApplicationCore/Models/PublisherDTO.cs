namespace KSE.GameStore.ApplicationCore.Models;

public class PublisherDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public string? Description { get; set; }
};