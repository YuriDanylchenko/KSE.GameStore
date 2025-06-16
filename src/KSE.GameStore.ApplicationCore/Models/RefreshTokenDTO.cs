namespace KSE.GameStore.ApplicationCore.Models;

public record RefreshTokenDTO(Guid UserId, string Token, DateTime Expires);
