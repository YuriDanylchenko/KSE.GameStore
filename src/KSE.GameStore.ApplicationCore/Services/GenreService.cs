using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.ApplicationCore.Models;
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
            throw new BadRequestException($"Genre ID must be a positive integer. Provided: {id}");

        return await _genreRepository.GetByIdAsync(id);
    }

    public async Task<Genre?> CreateGenreAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException("Genre name cannot be null, empty, or whitespace.");

        var existing = await _genreRepository
            .ListAsync(g => g.Name.ToLower() == name.ToLower());

        if (existing.Any())
            throw new BadRequestException($"A genre with the name '{name}' already exists.");

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
        if (id <= 0)
            throw new BadRequestException($"Genre ID must be a positive integer. Provided: {id}");

        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException("Genre name cannot be null, empty, or whitespace.");

        var genre = await GetGenreByIdAsync(id);

        if (genre == null)
            throw new NotFoundException($"Genre with ID {id} was not found.");

        genre.Name = name;

        _genreRepository.Update(genre);
        await _genreRepository.SaveChangesAsync();

        return genre;
    }

    public async Task<bool> DeleteGenreAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException($"Genre ID must be a positive integer. Provided: {id}");

        var genre = await GetGenreByIdAsync(id);

        if (genre == null)
            throw new NotFoundException($"Genre with ID {id} was not found.");

        _genreRepository.Delete(genre);
        await _genreRepository.SaveChangesAsync();

        return true;
    }
}