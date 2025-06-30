using AutoMapper;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.ApplicationCore.Infrastructure;
using KSE.GameStore.ApplicationCore.Models.Input;
using Microsoft.Extensions.Logging;

namespace KSE.GameStore.ApplicationCore.Services;

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
        IMapper mapper
    )

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
        var gameEntity = await _gameRepository.GetGameByIdAsync(id);

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

        var gameEntities = await _gameRepository.ListGamesAsync(pageNumber ?? 1, pageSize ?? 10);

        return _mapper.Map<List<GameDTO>>(gameEntities);
    }

    public async Task<GameDTO> CreateGameAsync(CreateGameDTO createGameDto)
    {
        if (await _gameRepository.ExistsAsync(g => g.Title.ToLower() == createGameDto.Title.ToLower()))
            throw new BadRequestException($"Game with title '{createGameDto.Title}' already exists.");

        var gameEntity = _mapper.Map<Game>(createGameDto);

        var publisherEntity = await _publisherRepository.GetByIdAsync(createGameDto.PublisherId);
        if (publisherEntity == null)
        {
            _logger.LogNotFound($"publishers/{createGameDto.PublisherId}");
            throw new NotFoundException($"Publisher with ID {createGameDto.PublisherId} not found.");
        }

        gameEntity.Publisher = publisherEntity;
        publisherEntity.Games ??= new List<Game>();
        publisherEntity.Games.Add(gameEntity);

        var genreIds = createGameDto.GenreIds;
        var genreEntities = (await _genreRepository.ListAllAsync(g => genreIds.Contains(g.Id))).ToList();
        if (genreEntities.Count != genreIds.Count)
        {
            var missingGenreIds = genreIds.Except(genreEntities.Select(g => g.Id)).ToList();
            _logger.LogNotFound($"genres/{string.Join(", ", missingGenreIds)}");
            throw new NotFoundException($"Genres not found: {string.Join(", ", missingGenreIds)}");
        }

        gameEntity.Genres = genreEntities;
        foreach (var genre in genreEntities)
        {
            genre.Games ??= new List<Game>();
            genre.Games.Add(gameEntity);
        }

        var platformIds = createGameDto.PlatformIds;
        var platformEntities = (await _platformRepository.ListAllAsync(p => platformIds.Contains(p.Id))).ToList();
        if (platformEntities.Count != platformIds.Count)
        {
            var missingPlatformIds = platformIds.Except(platformEntities.Select(p => p.Id)).ToList();
            _logger.LogNotFound($"platforms/{string.Join(", ", missingPlatformIds)}");
            throw new NotFoundException($"Platforms not found: {string.Join(", ", missingPlatformIds)}");
        }

        gameEntity.Platforms = platformEntities;
        foreach (var platform in platformEntities)
        {
            platform.Games ??= new List<Game>();
            platform.Games.Add(gameEntity);
        }

        var priceEntity = _mapper.Map<GamePrice>(createGameDto.PriceDto);
        gameEntity.Prices.Add(priceEntity);
        priceEntity.Game = gameEntity;

        if (createGameDto.RegionPermissionIds != null)
        {
            var regionIds = createGameDto.RegionPermissionIds;
            var regionEntities = (await _regionRepository.ListAllAsync(r => regionIds.Contains(r.Id))).ToList();

            if (regionIds.Any() && regionEntities.Count != regionIds.Count)
            {
                var missingRegionIds = regionIds.Except(regionEntities.Select(r => r.Id)).ToList();
                throw new NotFoundException($"Regions not found: {string.Join(", ", missingRegionIds)}");
            }

            gameEntity.RegionPermissions = regionEntities;
            foreach (var region in regionEntities)
            {
                region.Games ??= new List<Game>();
                region.Games.Add(gameEntity);
            }
        }
        else gameEntity.RegionPermissions = null;

        await _gameRepository.AddAsync(gameEntity);
        await _gameRepository.SaveChangesAsync();

        var createdGameEntity = await _gameRepository.GetGameByIdAsync(gameEntity.Id);
        return _mapper.Map<GameDTO>(createdGameEntity);
    }

    public async Task<GameDTO> UpdateGameAsync(UpdateGameDTO updateGameDto)
    {
        var exisingGameEntity = await _gameRepository.GetGameWithCollectionsByIdAsync(updateGameDto.Id);
        if (exisingGameEntity == null)
        {
            _logger.LogNotFound($"games/{updateGameDto.Id}");
            throw new NotFoundException($"Game with ID {updateGameDto.Id} not found.");
        }

        if (updateGameDto.Title != exisingGameEntity.Title &&
            await _gameRepository.ExistsAsync(g => g.Title.ToLower() == updateGameDto.Title.ToLower()))
            throw new BadRequestException($"Game with title '{updateGameDto.Title}' already exists.");

        _mapper.Map(updateGameDto, exisingGameEntity);

        var newPublisherEntity = await _publisherRepository.GetByIdAsync(updateGameDto.PublisherId);
        if (newPublisherEntity == null)
        {
            _logger.LogNotFound($"publishers/{updateGameDto.PublisherId}");
            throw new NotFoundException($"Publisher with ID {updateGameDto.PublisherId} not found.");
        }

        exisingGameEntity.Publisher = newPublisherEntity;

        var newGenreIds = updateGameDto.GenreIds;
        var newGenres = (await _genreRepository.ListAllAsync(g => newGenreIds.Contains(g.Id))).ToList();

        if (newGenres.Count != newGenreIds.Count)
        {
            var missingGenreIds = newGenreIds.Except(newGenres.Select(g => g.Id)).ToList();
            _logger.LogNotFound($"genres/{string.Join(", ", missingGenreIds)}");
            throw new NotFoundException($"Genres not found: {string.Join(", ", missingGenreIds)}");
        }

        exisingGameEntity.Genres = newGenres;

        var newPlatformIds = updateGameDto.PlatformIds;
        var newPlatforms = (await _platformRepository.ListAllAsync(p => newPlatformIds.Contains(p.Id))).ToList();

        if (newPlatforms.Count != newPlatformIds.Count)
        {
            var missingPlatformIds = newPlatformIds.Except(newPlatforms.Select(p => p.Id)).ToList();
            _logger.LogNotFound($"platforms/{string.Join(", ", missingPlatformIds)}");
            throw new NotFoundException($"Platforms not found: {string.Join(", ", missingPlatformIds)}");
        }

        exisingGameEntity.Platforms = newPlatforms;

        var currentPriceEntity = exisingGameEntity.Prices.FirstOrDefault(p => p.EndDate == null);
        if (currentPriceEntity != null)
            currentPriceEntity.EndDate = DateTime.UtcNow;

        var newPriceEntity = _mapper.Map<GamePrice>(updateGameDto.PriceDto);
        newPriceEntity.GameId = exisingGameEntity.Id;
        exisingGameEntity.Prices.Add(newPriceEntity);

        if (updateGameDto.RegionPermissionIds != null)
        {
            var newRegionIds = updateGameDto.RegionPermissionIds;
            var newRegions = (await _regionRepository.ListAllAsync(r => newRegionIds.Contains(r.Id))).ToList();

            if (newRegionIds.Any() && newRegions.Count != newRegionIds.Count)
            {
                var missingRegionIds = newRegionIds.Except(newRegions.Select(r => r.Id)).ToList();
                throw new NotFoundException($"Regions not found: {string.Join(", ", missingRegionIds)}");
            }

            exisingGameEntity.RegionPermissions = newRegions;
        }
        else exisingGameEntity.RegionPermissions = null;

        _gameRepository.Update(exisingGameEntity);
        await _gameRepository.SaveChangesAsync();

        var updatedGameEntity = await _gameRepository.GetGameByIdAsync(exisingGameEntity.Id);
        return _mapper.Map<GameDTO>(updatedGameEntity);
    }

    public async Task DeleteGameAsync(int id)
    {
        var gameEntity = await _gameRepository.GetGameByIdAsync(id);
        if (gameEntity == null)
        {
            _logger.LogNotFound($"games/{id}");
            throw new NotFoundException($"Game with ID {id} not found.");
        }

        _gameRepository.Delete(gameEntity);
        await _gameRepository.SaveChangesAsync();
    }

    public async Task<List<GameDTO>> GetGamesByGenreAsync(int genreId)
    {
        var genreEntity = await _genreRepository.GetByIdAsync(genreId);
        if (genreEntity == null)
        {
            _logger.LogNotFound($"games/genre/{genreId}");
            throw new NotFoundException($"Genre with ID {genreId} not found.");
        }

        var games = await _gameRepository.GetGamesByGenreAsync(genreId);

        return _mapper.Map<List<GameDTO>>(games.ToList());
    }

    public async Task<List<GameDTO>> GetGamesByPlatformAsync(int platformId)
    {
        var genreEntity = await _platformRepository.GetByIdAsync(platformId);
        if (genreEntity == null)
        {
            _logger.LogNotFound($"games/platform/{platformId}");
            throw new NotFoundException($"Platform with ID {platformId} not found.");
        }

        var gameEntities = await _gameRepository.GetGamesByPlatformAsync(platformId);

        return _mapper.Map<List<GameDTO>>(gameEntities);
    }
}