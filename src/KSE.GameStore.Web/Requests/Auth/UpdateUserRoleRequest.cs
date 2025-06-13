namespace KSE.GameStore.Web.Requests.Auth;
public record UpdateUserRoleRequest(Guid UserId, string RoleName);