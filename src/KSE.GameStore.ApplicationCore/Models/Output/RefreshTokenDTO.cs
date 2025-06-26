namespace KSE.GameStore.ApplicationCore.Models.Output;

public record RefreshTokenDTO(Guid UserId, string Token, DateTime Expires);
