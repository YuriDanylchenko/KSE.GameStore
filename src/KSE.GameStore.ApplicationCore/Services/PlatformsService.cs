using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;

namespace KSE.GameStore.ApplicationCore.Services;

public class PlatformsService(IRepository<Platform, int> repository, IMapper mapper) : IPlatformsService
{
    public async Task<List<PlatformDTO>> GetAllAsync()
    {
        var platforms = await repository.ListAsync();
        return mapper.Map<List<PlatformDTO>>(platforms);
    }

    public async Task<PlatformDTO> GetByIdAsync(int id)
    {
        var platform = await repository.GetByIdAsync(id);
        return platform is null
            ? throw new NotFoundException($"Platform with id {id} not found.")
            : mapper.Map<PlatformDTO>(platform);
    }

    public async Task<int> CreateAsync(string name)
    {
        var platform = new Platform { Name = name };
        await repository.AddAsync(platform);
        await repository.SaveChangesAsync();
        return platform.Id;
    }

    public async Task<bool> UpdateAsync(int id, string name)
    {
        var existing = await repository.GetByIdAsync(id) ??
                       throw new NotFoundException($"Platform with id {id} not found.");
        existing.Name = name;
        repository.Update(existing);
        await repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await repository.GetByIdAsync(id) ??
                       throw new NotFoundException($"Platform with id {id} not found.");
        repository.Delete(existing);
        await repository.SaveChangesAsync();
        return true;
    }
}