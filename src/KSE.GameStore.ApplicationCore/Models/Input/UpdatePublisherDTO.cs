namespace KSE.GameStore.ApplicationCore.Models.Input;

public record UpdatePublisherDTO(
    int Id,
    string Name,
    string? WebsiteUrl,
    string? Description
);