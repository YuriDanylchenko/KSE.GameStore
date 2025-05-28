using KSE.GameStore.ApplicationCore.Requests.Genre;

namespace KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.DataAccess.Entities;

public interface IGenreService
{
    Task<Genre?> GetGenreByIdAsync(int id);
    Task<Genre?> CreateGenreAsync(string name);
    Task<Genre?> UpdateGenreAsync(int id, string name);
    Task<bool> DeleteGenreAsync(int id);
}