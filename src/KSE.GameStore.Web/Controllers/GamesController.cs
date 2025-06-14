using AutoMapper;
using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.Web.Requests.Games;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IMapper _mapper;

    public GamesController(IGameService gameService, IMapper mapper)
    {
        _gameService = gameService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGames(int? pageNumber, int? pageSize)
    {
        var gameDtos = await _gameService.GetAllGamesAsync(pageNumber, pageSize);
        return Ok(gameDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGameById(int id)
    {
        var gameDto = await _gameService.GetGameByIdAsync(id);
        return Ok(gameDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest createGameRequest)
    {
        var createGameDto = _mapper.Map<CreateGameRequest, CreateGameDTO>(createGameRequest);
        var createdGameDto = await _gameService.CreateGameAsync(createGameDto);
        return Ok(createdGameDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGame([FromBody] UpdateGameRequest updateGameRequest)
    {
        var updateGameDto = _mapper.Map<UpdateGameRequest, UpdateGameDTO>(updateGameRequest);
        var updatedGameDto = await _gameService.UpdateGameAsync(updateGameDto);
        return Ok(updatedGameDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        await _gameService.DeleteGameAsync(id);
        return NoContent();
    }

    [HttpGet("/genre/{genreId:int}")]
    public async Task<IActionResult> GetGamesByGenre(int genreId)
    {
        var gameDtos = await _gameService.GetGamesByGenreAsync(genreId);
        return Ok(gameDtos);
    }

    [HttpGet("platform/{platformId:int}")]
    public async Task<IActionResult> GetGamesByPlatform(int platformId)
    {
        var gameDtos = await _gameService.GetGamesByPlatformAsync(platformId);
        return Ok(gameDtos);
    }
}