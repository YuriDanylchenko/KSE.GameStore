namespace KSE.GameStore.ApplicationCore.Models;
public record UserDTO(int Id, string FirstName, string LastName, string Email, RegionDTO? Region, List<RoleDTO> Roles);