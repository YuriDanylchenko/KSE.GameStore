using KSE.GameStore.ApplicationCore.Models.Output;

namespace KSE.GameStore.ApplicationCore.Services;

/// <summary>
/// Provides methods for user authentication.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the specified email, password, and region.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The password for the user.</param>
    /// <param name="regionId">The ID of the region the user belongs to.</param>
    /// <returns>An object containing user details if registration is successful, null otherwise.</returns>
    public Task<UserDTO?> RegisterUserAsync(string email, string password, int regionId);
    /// <summary>
    /// Logs in a user with the specified email and password.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The password for the user.</param>
    /// <returns>An object containing user details if login is successful, null otherwise.</returns>
    public Task<UserDTO?> LoginUserAsync(string email, string password);
    /// <summary>
    /// Logs out a user by invalidating their session token.
    /// </summary>
    /// <param name="token">The session token of the user.</param>
    /// <param name="uid">The unique identifier of the user.</param>
    /// <returns>true if logout is successful, false otherwise.</returns>
    public Task<bool> LogoutUserAsync(string token, Guid uid);
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>An object containing user details if found, null otherwise.</returns>
    public Task<UserDTO?> GetUserByEmailAsync(string email);
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>An object containing user details if found, null otherwise.</returns>
    public Task<UserDTO?> GetUserByIdAsync(Guid id);
    /// <summary>
    /// Updates the role of a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="role">The new role to assign to the user.</param>
    /// <returns>true if the role was updated successfully, false otherwise.</returns>
    public Task<bool?> UpdateUserRoleAsync(Guid userId, string role);
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="userDto">The user details for which to generate the token.</param>
    /// <returns>An object containing the access token and its expiration details.</returns>
    public AcessTokenDTO GenerateUserJwtToken(UserDTO userDto);
    /// <summary>
    /// Generates a refresh token for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom to generate the refresh token.</param>
    /// <returns>An object containing the refresh token and its expiration details.</returns>
    public Task<RefreshTokenDTO> GenerateRefreshTokenAsync(Guid userId);
}