using KSE.GameStore.DataAccess.Entities;
namespace KSE.GameStore.ApplicationCore.Interfaces;

public interface IPlatformsService
{
    Task<List<Platform>> GetAllAsync();
    Task<Platform?> GetByIdAsync(int id);
    Task<Platform> CreateAsync(string name);
    Task<bool> UpdateAsync(int id, string name);
    Task<bool> DeleteAsync(int id);
}