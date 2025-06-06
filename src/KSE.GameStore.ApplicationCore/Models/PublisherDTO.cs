namespace KSE.GameStore.ApplicationCore.Models;

public record PublisherDTO(
    int Id,
    string Name,
    string? WebsiteUrl,
    string? Description
);