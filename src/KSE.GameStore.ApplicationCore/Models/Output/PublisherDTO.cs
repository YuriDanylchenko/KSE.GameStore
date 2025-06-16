namespace KSE.GameStore.ApplicationCore.Models.Output;

public record PublisherDTO(
    int Id,
    string Name,
    string? WebsiteUrl,
    string? Description
);