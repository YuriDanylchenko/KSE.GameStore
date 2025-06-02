using KSE.GameStore.ApplicationCore.Domain;
using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.Web.Infrastructure;

namespace KSE.GameStore.Web.Services;

public class PlatformsService(IRepository<Platform, int> repository, ILogger<PlatformsService> logger) : IPlatformsService
{
    private readonly IRepository<Platform, int> _repository = repository;
    private readonly ILogger<PlatformsService> _logger = logger;

    public async Task<List<PlatformDto>> GetAllAsync()
    {
        var platforms = await _repository.ListAsync();
        return [.. platforms.Select(p => new PlatformDto(p.Id, p.Name))];
    }

    public async Task<PlatformDto> GetByIdAsync(int id)
    {
        var platform = await _repository.GetByIdAsync(id);
        if (platform is null)
        {
            _logger.LogNotFound($"Platform/{id}");
            throw new NotFoundException($"Platform with id {id} not found.");
        }
        return new PlatformDto(platform.Id, platform.Name);
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
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogNotFound($"Platform/{id}");
            throw new NotFoundException($"Platform with id {id} not found.");
        }
        existing.Name = name;
        _repository.Update(existing);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogNotFound($"Platform/{id}");
            throw new NotFoundException($"Platform with id {id} not found.");
        }
        _repository.Delete(existing);
        await _repository.SaveChangesAsync();
        return true;
    }
}