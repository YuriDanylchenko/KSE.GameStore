using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KSE.GameStore.ApplicationCore.Services;

public class AuthService(
    IRepository<User, Guid> userRepository,
    IRepository<Role, int> roleRepository,
    IRepository<Region, int> regionRepository,
    IRepository<RefreshToken, int> refreshTokenRepository,
    IMapper mapper,
    string token) : IAuthService
{
    public async Task<UserDTO?> RegisterUserAsync(string email, string password, int regionId)
    {
        var trimmedEmail = email.Trim().ToLower();

        var existingUser = await userRepository
            .ListAllAsync(u => u.Email == trimmedEmail);

        if (existingUser.Any())
            return null;

        var salt = Guid.NewGuid().ToString("N");

        var region = await regionRepository.GetByIdAsync(regionId)
            ?? throw new ArgumentException("Region not found.");
        var user = new User
        {
            Name = trimmedEmail,
            Email = trimmedEmail,
            HashedPassword = HashPassword(password, salt),
            PasswordSalt = salt,
            Region = region
        };

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();
        return mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO?> LoginUserAsync(string email, string password)
    {
        var trimmedEmail = email.Trim().ToLower();

        var users = await userRepository.ListAllAsync(u => u.Email == trimmedEmail);
        var user = users.FirstOrDefault();

        if (user == null)
            return null;

        var hashedPassword = HashPassword(password, user.PasswordSalt);

        return hashedPassword != user.HashedPassword ? null : mapper.Map<UserDTO>(user);
    }

    public async Task<bool> LogoutUserAsync(string token, Guid uid)
    {
        var tokens = await refreshTokenRepository.ListAllAsync(
            rt => rt.UserId == uid && rt.Token == token && !rt.IsRevoked
        );
        var refreshToken = tokens.FirstOrDefault();
        if (refreshToken == null)
            return false;

        refreshToken.IsRevoked = true;
        refreshTokenRepository.Update(refreshToken);
        await refreshTokenRepository.SaveChangesAsync();
        return true;
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid id)
    {
        var users = await userRepository.ListAllAsync(
            u => u.Id == id
        );
        var user = users.FirstOrDefault();
        return user == null ? null : mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO?> GetUserByEmailAsync(string email)
    {
        var trimmedEmail = email.Trim().ToLower();
        var users = await userRepository.ListAllAsync(u => u.Email == trimmedEmail);
        var user = users.FirstOrDefault();
        return user == null ? null : mapper.Map<UserDTO>(user);
    }

    public async Task<bool?> UpdateUserRoleAsync(Guid userId, string role)
    {
        var users = await userRepository.ListAllAsync(u => u.Id == userId);
        var user = users.FirstOrDefault();
        if (user == null) return null;

        user.UserRoles.Clear();
        var roles = await roleRepository.ListAllAsync(r => r.Name == role);
        var roleEntity = roles.FirstOrDefault();
        if (roleEntity != null)
        {
            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleEntity.Id });
        }
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
        return true;
    }

    public AcessTokenDTO GenerateUserJwtToken(UserDTO user)
    {
        var userEntity = new User
        {
            Id = user.Id,
            Email = user.Email,
            Region = new Region { Id = user.Region?.Id ?? 0 },
            UserRoles = [.. user.Roles.Select(r => new UserRole
            {
                Role = new Role { Name = r.Name }
            })]
        };
        var roles = userEntity.UserRoles.Select(ur => ur.Role.Name).ToList();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userEntity.Id.ToString()),
            new(ClaimTypes.Email, userEntity.Email),
            new (ClaimTypes.Role, "Admin")
        };

        //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        const int expiration = 30;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiration),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token)),
                SecurityAlgorithms.HmacSha512Signature)
        }; 

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        Console.WriteLine($"JWT Token: {tokenHandler.WriteToken(securityToken)}");

        return new AcessTokenDTO
        (
            tokenHandler.WriteToken(securityToken),
            securityToken.ValidTo
        );
    }

    public static string HashPassword(string password, string salt)
    {
        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashedPassword = HMACSHA512.HashData(saltBytes, passwordBytes);

        return Convert.ToBase64String(hashedPassword);
    }

    public static TokenValidationParameters CreateTokenValidationParameters(string jwtKey)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            LifetimeValidator = (_, expires, _, _) =>
            {
                if (expires == null)
                    return false;

                return expires > DateTime.UtcNow;
            }
        };
    }
}