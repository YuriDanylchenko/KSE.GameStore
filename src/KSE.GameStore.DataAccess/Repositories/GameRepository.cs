using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.DataAccess.Repositories;

public class GameRepository(GameStoreDbContext context) : Repository<Game, int>(context), IGameRepository
{
    public async Task<List<GameDTO>> GetGamesByPlatformAsync(int platformId)
    {
        var query = from game in _dbSet
                    from platform in game.Platforms
                    where platform.Id == platformId
                    select new GameDTO
                    {
                        Id = game.Id,
                        Title = game.Title,
                        Description = game.Description,
                        Publisher = game.Publisher.Name,
                        Genres = game.Genres.Select(genre => new GenreDTO
                        (
                            genre.Id,
                            genre.Name
                        )).ToList(),
                        Platforms = game.Platforms.Select(platform => new PlatformDTO
                        (
                            platform.Id,
                            platform.Name
                        )).ToList(),
                        Price = game.Prices
                            .OrderByDescending(price => price.StartDate)
                            .Select(price => new GamePriceDTO
                            (
                                price.Id,
                                price.Value,
                                price.Stock
                            ))
                            .FirstOrDefault()!,
                        RegionPermissions = game.RegionPermissions != null
                            ? game.RegionPermissions.Select(region => new RegionDTO
                            (
                                region.Id,
                                region.Name,
                                region.Code
                            )).ToList()
                            : new List<RegionDTO>()
                    };

        return await query.ToListAsync();
    }
}