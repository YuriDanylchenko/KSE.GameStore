using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;

namespace KSE.GameStore.Web.Services;

public class PlatformsService(IRepository<Platform, int> repository) : IPlatformsService
{
    public async Task<List<Platform>> GetAllAsync() => (await repository.ListAsync()).ToList();

    public async Task<Platform?> GetByIdAsync(int id) => await repository.GetByIdAsync(id);

    public async Task<Platform> CreateAsync(string name)
    {
        var platform = new Platform { Name = name }; 
        await repository.AddAsync(platform); 
        await repository.SaveChangesAsync();
        return platform;
    }

    public async Task<bool> UpdateAsync(int id, string name)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing == null) return false;
        existing.Name = name; 
        repository.Update(existing);
        await repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing == null) return false;
        repository.Delete(existing);
        await repository.SaveChangesAsync();
        return true;
    }
}