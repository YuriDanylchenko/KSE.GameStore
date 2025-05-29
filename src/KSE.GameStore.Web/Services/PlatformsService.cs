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

    public async Task<List<Platform>> GetAllAsync() => [.. (await _repository.ListAsync())];

    public async Task<Platform?> GetByIdAsync(int id)
    {
        var platform = await _repository.GetByIdAsync(id);
        if (platform == null)
        {
            _logger.LogNotFound($"Platform/{id}");
            throw new NotFoundException($"Platform with id {id} not found.");
        }
        return platform;
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
        if (existing == null)
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