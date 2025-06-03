using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.DataAccess.Repositories;
public interface IGameRepository : IRepository<Game, int>
{
    Task<List<GameDTO>> GetGamesByPlatformAsync(int platformId); 
}