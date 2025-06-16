using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.Web.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var userDto = await authService.LoginUserAsync(request.Email, request.Password);
        if (userDto is null)
            return Unauthorized(new { message = "Invalid email or password." });

        var users = await authService.GetUserByEmailAsync(request.Email);
        if (users is null)
            return Unauthorized(new { message = "Invalid email or password." });

        var userEntity = users;
        var tokenResult = authService.GenerateUserJwtToken(userEntity);

        var refreshToken = await authService.GenerateRefreshTokenAsync(userEntity.Id);

        return Ok(new
        {
            token = tokenResult.Token,
            expires = tokenResult.Expiration,
            refreshToken = refreshToken.Token,
            refreshTokenExpires = refreshToken.Expires,
            user = userDto
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var userDto = await authService.RegisterUserAsync(request.Email, request.Password, 1);
        if (userDto is null)
            return BadRequest(new { message = "User already exists." });
        return CreatedAtAction(nameof(Register), new { email = userDto.Email }, userDto);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] string token)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
        if (userIdClaim == null)
            return Unauthorized(new { Message = "User not found" });
        var userId = Guid.Parse(userIdClaim.Value);

        var success = await authService.LogoutUserAsync(token, userId);
        if (!success)
            return BadRequest(new { Message = "Invalid refresh token" });
        return Ok(new { Message = "Logout successful" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(uid))
            return BadRequest(new { message = "Invalid user ID." });
        var userDto = await authService.GetUserByIdAsync(Guid.Parse(uid));
        if (userDto is null)
            return NotFound(new { message = "User not found." });

        return Ok(userDto);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var userDto = await authService.GetUserByIdAsync(request.Id);
        if (userDto is null)
            return NotFound(new { message = "User not found." });
        var tokenResult = authService.GenerateUserJwtToken(userDto);
        return Ok(new
        {
            token = tokenResult.Token,
            expires = tokenResult.Expiration,
            user = userDto
        });
    }

    [HttpPut("update-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleRequest request)
    {
        var userDto = await authService.GetUserByIdAsync(request.UserId);
        if (userDto is null)
            return NotFound(new { message = "User not found." });
        var updated = await authService.UpdateUserRoleAsync(request.UserId, request.RoleName);
        if (updated is null)
            return BadRequest(new { message = "Failed to update user role." });

        return Ok(new { message = "User role updated successfully." });
    }
}