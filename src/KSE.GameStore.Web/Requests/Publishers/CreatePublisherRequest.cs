namespace KSE.GameStore.Web.Requests.Publishers;

public record CreatePublisherRequest(string Name, string? Description, string? WebsiteUrl);