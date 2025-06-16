using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using Moq;
using System.Linq.Expressions;

namespace KSE.GameStore.Tests.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IRepository<User, Guid>> _userRepo = new();
    private readonly Mock<IRepository<Role, int>> _roleRepo = new();
    private readonly Mock<IRepository<Region, int>> _regionRepo = new();
    private readonly Mock<IRepository<RefreshToken, int>> _refreshTokenRepo = new();
    private readonly Mock<IMapper> _mapper = new();

    private AuthService CreateService() =>
        new(_userRepo.Object, _roleRepo.Object, _regionRepo.Object, _refreshTokenRepo.Object, _mapper.Object, Guid.NewGuid().ToString());

    [Fact]
    public async Task RegisterUserAsync_Returns400_WhenUserExists()
    {
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([new() { Email = "test@test.com", Region = new Region { Id = 1, Name = "Default", Code = "DR" } }]);

        var service = CreateService();
        await Assert.ThrowsAsync<ServerException>(() =>
            service.RegisterUserAsync("test@test.com", "pass", 1));
    }

    [Fact]
    public async Task RegisterUserAsync_Throws_WhenRegionNotFound()
    {
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([]);
        _regionRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(null as Region);

        var service = CreateService();
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.RegisterUserAsync("test@test.com", "pass", 1));
    }

    [Fact]
    public async Task RegisterUserAsync_ReturnsUserDTO_WhenSuccess()
    {
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([]);
        _regionRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Region { Id = 1, Name = "EU", Code = "EU" });
        _mapper.Setup(m => m.Map<UserDTO>(It.IsAny<User>()))
            .Returns(new UserDTO(Guid.NewGuid(), "test@test.com", string.Empty, string.Empty, null, []));

        var service = CreateService();
        var result = await service.RegisterUserAsync("test@test.com", "pass", 1);

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task LoginUserAsync_Returns404_WhenUserNotFound()
    {
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([]);

        var service = CreateService();
        await Assert.ThrowsAsync<ServerException>(() =>
             service.LoginUserAsync("test@test.com", "pass"));
    }

    [Fact]
    public async Task LoginUserAsync_ReturnsNull_WhenPasswordIncorrect()
    {
        var user = new User
        {
            Email = "test@test.com",
            PasswordSalt = "salt",
            HashedPassword = "wrong",
            Region = new Region { Id = 1, Name = "Default", Code = "DR" }
        };
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([user]);

        var service = CreateService();
        var result = await service.LoginUserAsync("test@test.com", "pass");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginUserAsync_ReturnsUserDTO_WhenSuccess()
    {
        var salt = Guid.NewGuid().ToString("N");
        var password = "pass";
        var hashed = AuthService.HashPassword(password, salt);
        var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", PasswordSalt = salt, HashedPassword = hashed, Region = new() { Id = 1, Name = "Default", Code = "DR" } };
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([user]);
        _mapper.Setup(m => m.Map<UserDTO>(user))
            .Returns(new UserDTO(Guid.NewGuid(), "test@test.com", string.Empty, string.Empty, null, []));

        var service = CreateService();
        var result = await service.LoginUserAsync("test@test.com", password);

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task LogoutUserAsync_ReturnsFalse_WhenTokenNotFound()
    {
        _refreshTokenRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync([]);

        var service = CreateService();
        var result = await service.LogoutUserAsync("token", Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task LogoutUserAsync_ReturnsTrue_WhenTokenRevoked()
    {
        var token = new RefreshToken { Id = 1, Token = "token", UserId = Guid.NewGuid(), IsRevoked = false };
        _refreshTokenRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
            .ReturnsAsync([token]);

        var service = CreateService();
        var result = await service.LogoutUserAsync("token", token.UserId);

        Assert.True(result);
        Assert.True(token.IsRevoked);
        _refreshTokenRepo.Verify(r => r.Update(token), Times.Once);
        _refreshTokenRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenNotFound()
    {
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([]);

        var service = CreateService();
        var result = await service.GetUserByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUserDTO_WhenFound()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "test@test.com", Region = new() { Id = 1, Name = "Default", Code = "DR" } };
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([user]);
        _mapper.Setup(m => m.Map<UserDTO>(user))
            .Returns(new UserDTO(userId, "test@test.com", string.Empty, string.Empty, null, new List<RoleDTO>()));

        var service = CreateService();
        var result = await service.GetUserByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsNull_WhenNotFound()
    {
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([]);

        var service = CreateService();
        var result = await service.GetUserByEmailAsync("test@test.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUserDTO_WhenFound()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@test.com",
            Region = new Region { Id = 1, Name = "Default", Code = "DR" }
        };
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([user]);
        _mapper.Setup(m => m.Map<UserDTO>(user))
            .Returns(new UserDTO(userId, "test@test.com", string.Empty, string.Empty, null, new List<RoleDTO>()));

        var service = CreateService();
        var result = await service.GetUserByEmailAsync("test@test.com");

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ReturnsNull_WhenUserNotFound()
    {
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([]);

        var service = CreateService();
        var result = await service.UpdateUserRoleAsync(Guid.NewGuid(), "Admin");

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ReturnsTrue_WhenRoleUpdated()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            UserRoles = [],
            Region = new Region { Id = 1, Name = "Default", Code = "DR" }
        };
        var role = new Role { Id = 2, Name = "Admin" };
        _userRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync([user]);
        _roleRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Role, bool>>>()))
            .ReturnsAsync([role]);

        var service = CreateService();
        var result = await service.UpdateUserRoleAsync(userId, "Admin");

        Assert.True(result);
        Assert.Single(user.UserRoles);
        Assert.Equal(role.Id, user.UserRoles.First().RoleId);
        _userRepo.Verify(r => r.Update(user), Times.Once);
        _userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}