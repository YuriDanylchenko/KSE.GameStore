using AutoMapper;
using AutoMapper.QueryableExtensions;
using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Requests.Games;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.Web.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.Web.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IRepository<Genre, int> _genreRepository;
    private readonly IRepository<Platform, int> _platformRepository;
    private readonly IRepository<Region, int> _regionRepository;
    private readonly IRepository<Publisher, int> _publisherRepository;
    private readonly ILogger<GameService> _logger;
    private readonly IMapper _mapper;

    public GameService(IGameRepository gameRepository,
        IRepository<Genre, int> genreRepository,
        IRepository<Platform, int> platformRepository,
        IRepository<Region, int> regionRepository,
        IRepository<Publisher, int> publisherRepository,
        ILogger<GameService> logger,
        IMapper mapper)

    {
        _gameRepository = gameRepository;
        _genreRepository = genreRepository;
        _platformRepository = platformRepository;
        _regionRepository = regionRepository;
        _publisherRepository = publisherRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<GameDTO> GetGameByIdAsync(int id)
    {
        var gameEntity = await _gameRepository.GetByIdAsync(
            id,
            include: q => q
                .Include(g => g.Publisher)
                .Include(g => g.Genres)
                .Include(g => g.Platforms)
                .Include(g => g.Prices)
                .Include(g => g.RegionPermissions)
        );

        if (gameEntity == null)
        {
            _logger.LogNotFound($"games/{id}");
            throw new NotFoundException($"Game with ID {id} not found.");
        }

        return _mapper.Map<GameDTO>(gameEntity);
    }

    public async Task<List<GameDTO>> GetAllGamesAsync(int? pageNumber, int? pageSize)
    {
        if (pageNumber.HasValue && pageNumber <= 0)
            throw new BadRequestException($"Page number must be a positive integer. Provided: {pageNumber}");

        if (pageSize.HasValue && pageSize <= 0)
            throw new BadRequestException($"Page size must be a positive integer. Provided: {pageSize}");

        var gameEntities = await _gameRepository.ListAsync(
            pageNumber ?? 1,
            pageSize ?? 10,
            include: q => q
                .Include(g => g.Publisher)
                .Include(g => g.Genres)
                .Include(g => g.Platforms)
                .Include(g => g.Prices)
                .Include(g => g.RegionPermissions)
        );

        return _mapper.Map<List<GameDTO>>(gameEntities);
    }

    public async Task<GameDTO> CreateGameAsync(CreateGameRequest createGameRequest)
    {
        // bad request for invalid data (fluent validation)

        if (await _gameRepository.ExistsAsync(g => g.Title.ToLower() == createGameRequest.Title.ToLower()))
            throw new BadRequestException($"Game with title '{createGameRequest.Title}' already exists.");

        var gameEntity = _mapper.Map<Game>(createGameRequest);

        var publisherEntity = await _publisherRepository.GetByIdAsync(createGameRequest.PublisherId);
        if (publisherEntity == null)
        {
            _logger.LogNotFound($"publishers/{createGameRequest.PublisherId}");
            throw new NotFoundException($"Publisher with ID {createGameRequest.PublisherId} not found.");
        }

        gameEntity.Publisher = publisherEntity;

        var genreEntities = await _genreRepository.ListAsync(g => createGameRequest.GenreIds.Contains(g.Id));
        gameEntity.Genres = genreEntities.ToList();

        var platformEntities = await _platformRepository.ListAsync(p => createGameRequest.PlatformIds.Contains(p.Id));
        gameEntity.Platforms = platformEntities.ToList();

        var priceEntity = _mapper.Map<GamePrice>(createGameRequest.Price);
        priceEntity.Game = gameEntity;
        gameEntity.Prices = new List<GamePrice> { priceEntity };

        if (createGameRequest.RegionPermissionIds != null)
        {
            var regionEntities =
                await _regionRepository.ListAsync(r => createGameRequest.RegionPermissionIds.Contains(r.Id));
            gameEntity.RegionPermissions = regionEntities.ToList();
        }
        else gameEntity.RegionPermissions = new List<Region>();

        await _gameRepository.AddAsync(gameEntity);
        await _gameRepository.SaveChangesAsync();

        var dto = await _gameRepository
            .Query()
            .Where(g => g.Id == gameEntity.Id)
            .ProjectTo<GameDTO>(_mapper.ConfigurationProvider)
            .SingleAsync();

        return dto;
    }

    public async Task<GameDTO> UpdateGameAsync(UpdateGameRequest updateGameRequest)
    {
        // bad request for invalid data (fluent validation)

        var gameEntity = await _gameRepository.GetByIdAsync(
            updateGameRequest.Id,
            include: q => q
                .Include(g => g.Genres)
                .Include(g => g.Platforms)
                .Include(g => g.RegionPermissions)
                .Include(g => g.Prices)
        );
        if (gameEntity == null)
        {
            _logger.LogNotFound($"games/{updateGameRequest.Id}");
            throw new NotFoundException($"Game with ID {updateGameRequest.Id} not found.");
        }

        if (updateGameRequest.Title != gameEntity.Title &&
            await _gameRepository.ExistsAsync(g => g.Title.ToLower() == updateGameRequest.Title.ToLower()))
            throw new BadRequestException($"Game with title '{updateGameRequest.Title}' already exists.");

        _mapper.Map(updateGameRequest, gameEntity);

        var publisherEntity = await _publisherRepository.GetByIdAsync(updateGameRequest.PublisherId);
        if (publisherEntity == null)
        {
            _logger.LogNotFound($"publishers/{updateGameRequest.PublisherId}");
            throw new NotFoundException($"Publisher with ID {updateGameRequest.PublisherId} not found.");
        }

        gameEntity.Publisher = publisherEntity;

        gameEntity.Genres.Clear();
        var genreEntities = await _genreRepository.ListAsync(g => updateGameRequest.GenreIds.Contains(g.Id));
        gameEntity.Genres = genreEntities.ToList();

        gameEntity.Platforms.Clear();
        var platformEntities = await _platformRepository.ListAsync(p => updateGameRequest.PlatformIds.Contains(p.Id));
        gameEntity.Platforms = platformEntities.ToList();

        var currentPriceEntity = gameEntity.Prices.FirstOrDefault(p => p.EndDate == null);
        if (currentPriceEntity != null)
            currentPriceEntity.EndDate = DateTime.UtcNow;

        var newPriceEntity = _mapper.Map<GamePrice>(updateGameRequest.Price);
        newPriceEntity.Game = gameEntity;
        gameEntity.Prices.Add(newPriceEntity);

        gameEntity.RegionPermissions?.Clear();
        if (updateGameRequest.RegionPermissionIds != null)
        {
            var regionEntities =
                await _regionRepository.ListAsync(r => updateGameRequest.RegionPermissionIds.Contains(r.Id));
            gameEntity.RegionPermissions = regionEntities.ToList();
        }

        _gameRepository.Update(gameEntity);
        await _gameRepository.SaveChangesAsync();

        var dto = await _gameRepository
            .Query()
            .Where(g => g.Id == gameEntity.Id)
            .ProjectTo<GameDTO>(_mapper.ConfigurationProvider)
            .SingleAsync();

        return dto;
    }

    public async Task DeleteGameAsync(int id)
    {
        var gameEntity = await _gameRepository.GetByIdAsync(id);
        if (gameEntity == null)
        {
            _logger.LogNotFound($"games/{id}");
            throw new NotFoundException($"Game with ID {id} not found.");
        }

        _gameRepository.Delete(gameEntity);
        await _gameRepository.SaveChangesAsync();
    }

    public async Task<List<GameDTO>> GetGamesByPlatformAsync(int platformId)
    {
        _ = await _platformRepository.GetByIdAsync(platformId) 
            ?? throw new NotFoundException($"Platform with ID {platformId} not found.");

        return await _gameRepository.GetGamesByPlatformAsync(platformId);
    }
}