using KSE.GameStore.ApplicationCore.Models;
using Microsoft.IdentityModel.Tokens;

namespace KSE.GameStore.ApplicationCore.Services;
public interface IAuthService
{
    public Task<UserDTO?> RegisterUserAsync(string email, string password, int regionId);

    public Task<UserDTO?> LoginUserAsync(string email, string password);

    public Task<bool> LogoutUserAsync(string token, Guid uid);

    public Task<UserDTO?> GetUserByEmailAsync(string email);

    public Task<UserDTO?> GetUserByIdAsync(int id);

    public Task<bool?> UpdateUserRoleAsync(int userId, string role);
}