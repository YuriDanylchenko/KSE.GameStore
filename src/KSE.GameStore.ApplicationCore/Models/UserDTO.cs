namespace KSE.GameStore.ApplicationCore.Models;
public record UserDTO(int Id, string Email, string HashedPassword, string PasswordSalt, RegionDTO? Region, List<RoleDTO> Roles);