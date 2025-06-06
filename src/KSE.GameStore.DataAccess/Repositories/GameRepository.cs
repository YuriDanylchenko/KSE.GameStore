using System.Linq.Expressions;
using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.DataAccess.Repositories;

public class GameRepository(GameStoreDbContext context) : Repository<Game, int>(context), IGameRepository
{
    public async Task<Game?> GetGameWithCollectionsByIdAsync(int id)
    {
        return await _dbSet
            .Include(g => g.Publisher)
            .Include(g => g.Genres)     
            .Include(g => g.Platforms)  
            .Include(g => g.Prices)             
            .Include(g => g.RegionPermissions) 
            .SingleOrDefaultAsync(g => g.Id == id);
    }
    
    public async Task<List<Game>> GetGamesByPlatformAsync(int platformId)
    {
        var query = from game in _dbSet
                    from platform in game.Platforms
                    where platform.Id == platformId
                    select new Game
                    {
                        Id = game.Id,
                        Title = game.Title,
                        Description = game.Description,
                        PublisherId = game.PublisherId,
                        CreatedAt = game.CreatedAt,
                        UpdatedAt = game.UpdatedAt,
                        Publisher = game.Publisher,
                        Genres = game.Genres,
                        Platforms = game.Platforms,
                        Prices = game.Prices,
                        RegionPermissions = game.RegionPermissions
                    };

        return await query.ToListAsync();
    }
    
    public async Task<Game?> GetGameByIdAsync(int id)
    {
        var query = from game in _dbSet
            where game.Id == id
            select new Game
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                PublisherId = game.PublisherId,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                Publisher = game.Publisher,
                Genres = game.Genres,
                Platforms = game.Platforms,
                Prices = game.Prices,
                RegionPermissions = game.RegionPermissions
            };
        return await query.FirstOrDefaultAsync();
    }
    
    public async Task<List<Game>> ListGamesAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = from game in _dbSet
            orderby game.Id
            select new Game
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                PublisherId = game.PublisherId,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                Publisher = game.Publisher,
                Genres = game.Genres,
                Platforms = game.Platforms,
                Prices = game.Prices,
                RegionPermissions = game.RegionPermissions
            };

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public Task<bool> ExistsAsync(Expression<Func<Game, bool>> predicate) => _dbSet.AnyAsync(predicate);
    public IQueryable<Game> Query() => _dbSet.AsNoTracking();
}