namespace KSE.GameStore.Web.Requests.Publishers;

public record UpdatePublisherRequest(
    int Id,
    string Name,
    string WebsiteUrl,
    string Description
);