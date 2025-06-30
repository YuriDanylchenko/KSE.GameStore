using AutoMapper;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;

namespace KSE.GameStore.ApplicationCore.Services;

public class GenreService : IGenreService
{
    private readonly IMapper _mapper;
    private readonly IRepository<Genre, int> _genreRepository;

    public GenreService(IMapper mapper, IRepository<Genre, int> genreRepository)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<Genre?> GetGenreByIdAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);

        if (genre == null)
            throw new NotFoundException($"Genre with ID {id} was not found.");

        return genre;
    }

    public async Task<Genre?> CreateGenreAsync(string name)
    {
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
        var genre = await GetGenreByIdAsync(id);

        if (genre == null)
            throw new NotFoundException($"Genre with ID {id} was not found.");

        _genreRepository.Delete(genre);
        await _genreRepository.SaveChangesAsync();

        return true;
    }

    public async Task<List<GenreDTO>> GetAllGenresAsync(int? pageNumber, int? pageSize)
    {
        if (pageNumber is <= 0)
            throw new BadRequestException($"Page number must be a positive integer. Provided: {pageNumber}");

        if (pageSize is <= 0)
            throw new BadRequestException($"Page size must be a positive integer. Provided: {pageSize}");

        var genresEntities = await _genreRepository.ListAsync(pageNumber ?? 1, pageSize ?? 10);

        return _mapper.Map<List<GenreDTO>>(genresEntities);
    }
}