using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.DataAccess.Repositories;

public class GameRepository(GameStoreDbContext context) : Repository<Game, int>(context), IGameRepository
{
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
                        Prices = game.Prices
                    };

        return await query.ToListAsync();
    }
}