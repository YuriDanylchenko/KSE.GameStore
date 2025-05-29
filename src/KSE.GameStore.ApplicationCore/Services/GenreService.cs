using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;

namespace KSE.GameStore.ApplicationCore.Services;

public class GenreService : IGenreService
{
    private readonly IRepository<Genre, int> _genreRepository;

    public GenreService(IRepository<Genre, int> genreRepository)
    {
        _genreRepository = genreRepository;
    }


    public async Task<Genre?> GetGenreByIdAsync(int id)
    {
        if (id <= 0)
            return null;
        
        return await _genreRepository.GetByIdAsync(id);
    }

    public async Task<Genre?> CreateGenreAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;
        
        var existing = await _genreRepository
            .ListAsync(g => g.Name.ToLower() == name.ToLower());
        
        if (existing.Any())
            return null;
        
        var genre = new Genre
        {
            Name = name,
            Games = new List<Game>()
        };

        await _genreRepository.AddAsync(genre);
        await _genreRepository.SaveChangesAsync();

        return genre;
    }

    public async Task<Genre?> UpdateGenreAsync(int id, string name)
    {
        if (id <= 0 || string.IsNullOrWhiteSpace(name))
            return null;
        
        var genre = await GetGenreByIdAsync(id);
        
        if (genre == null)
            return null;
        
        genre.Name = name;
        
        _genreRepository.Update(genre);
        await _genreRepository.SaveChangesAsync();

        return genre;
    }

    public async Task<bool> DeleteGenreAsync(int id)
    {
        if (id <= 0)
            return false;
        
        var genre = await GetGenreByIdAsync(id);
        
        if (genre == null)
            return false;
        
        _genreRepository.Delete(genre);
        await _genreRepository.SaveChangesAsync();

        return true;
    }
}