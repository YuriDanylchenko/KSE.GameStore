using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.DataAccess.Repositories;
public interface IGameRepository : IRepositoryOld<Game, int>
{
    Task<List<Game>> GetGamesByPlatformAsync(int platformId); 
}