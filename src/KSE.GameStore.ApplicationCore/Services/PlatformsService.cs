using KSE.GameStore.ApplicationCore.Models.Output;
using AutoMapper;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;

namespace KSE.GameStore.ApplicationCore.Services;

public class PlatformsService : IPlatformsService
{
    private readonly IRepository<Platform, int> _repository;
    private readonly IMapper _mapper;

    public PlatformsService(IRepository<Platform, int> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<PlatformDTO>> GetAllAsync()
    {
        var platforms = await _repository.ListAsync();
        return _mapper.Map<List<PlatformDTO>>(platforms);
    }

    public async Task<PlatformDTO> GetByIdAsync(int id)
    {
        var platform = await _repository.GetByIdAsync(id);
        return platform is null
            ? throw new NotFoundException($"Platform with id {id} not found.")
            : _mapper.Map<PlatformDTO>(platform);
    }

    public async Task<int> CreateAsync(string name)
    {
        var platform = new Platform { Name = name };
        await _repository.AddAsync(platform);
        await _repository.SaveChangesAsync();
        return platform.Id;
    }

    public async Task<bool> UpdateAsync(int id, string name)
    {
        var existing = await _repository.GetByIdAsync(id) ??
                       throw new NotFoundException($"Platform with id {id} not found.");
        existing.Name = name;
        _repository.Update(existing);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id) ??
                       throw new NotFoundException($"Platform with id {id} not found.");
        _repository.Delete(existing);
        await _repository.SaveChangesAsync();
        return true;
    }
}