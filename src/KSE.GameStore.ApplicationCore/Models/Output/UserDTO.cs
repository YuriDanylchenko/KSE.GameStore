namespace KSE.GameStore.ApplicationCore.Models.Output;

public record UserDTO
(
    Guid Id,
    string Email,
    string HashedPassword,
    string PasswordSalt,
    RegionDTO? Region,
    List<RoleDTO> Roles
);