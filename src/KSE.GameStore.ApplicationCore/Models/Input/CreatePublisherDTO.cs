namespace KSE.GameStore.ApplicationCore.Models.Input;

public record CreatePublisherDTO(
    string Name,
    string? WebsiteUrl,
    string? Description
);