using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.DataAccess.Repositories;

public class GameRepository(GameStoreDbContext context) : Repository<Game, int>(context), IGameRepository
{
    public async Task<List<GameDTO>> GetGamesByPlatformAsync(int platformId)
    {
        var sql = @$"
            SELECT g.id, g.title, g.description, g.publisher_id, g.created_at, g.updated_at
            FROM games g
            INNER JOIN game_platforms gp ON g.id = gp.game_id
            WHERE gp.platform_id = {platformId}";

        var games = await _context.Games
            .FromSqlRaw(sql)
            .Select(g => new GameDTO
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Publisher = g.Publisher.Name,
                Genres = g.Genres.Select(genre => new GenreDTO
                (
                    genre.Id,
                    genre.Name
                )).ToList(),
                Platforms = g.Platforms.Select(platform => new PlatformDTO
                (
                    platform.Id,
                    platform.Name
                )).ToList(),
                Price = g.Prices.Select(price => new GamePriceDTO
                (
                    price.Id,
                    price.Value,
                    price.Stock
                )).FirstOrDefault()!,
                RegionPermissions = g.RegionPermissions != null
                    ? g.RegionPermissions.Select(region => new RegionDTO
                    (
                        region.Id,
                        region.Name,
                        region.Code
                    )).ToList()
                    : new()
            })
            .ToListAsync();

        return games;
    }
}