using System.Linq.Expressions;
using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.DataAccess.Repositories;

public interface IGameRepository : IRepository<Game, int>
{
    Task<Game?> GetGameWithCollectionsByIdAsync(int id);
    Task<List<Game>> GetGamesByGenreAsync(int genreId);
    Task<List<Game>> GetGamesByPlatformAsync(int platformId);
    Task<Game?> GetGameByIdAsync(int id);
    Task<List<Game>> ListGamesAsync(int pageNumber = 1, int pageSize = 10);
    Task<bool> ExistsAsync(Expression<Func<Game, bool>> predicate);
    IQueryable<Game> Query();
}