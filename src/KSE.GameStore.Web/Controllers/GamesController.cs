using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.Web.Requests.Games;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAllGenres(int? pageNumber, int? pageSize)
    {
        var gameDto = await _gameService.GetAllGamesAsync(pageNumber, pageSize);
        return Ok(gameDto);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetGameById(int id)
    {
        var gameDto = await _gameService.GetGameByIdAsync(id);
        return Ok(gameDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest createGameRequest)
    {
        var gameDto = _mapper.Map<CreateGameRequest, GameDTO>(createGameRequest);
        var createdGameDto = await _gameService.CreateGameAsync(gameDto);
        return Ok(createdGameDto);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateGame([FromBody] UpdateGameRequest updateGameRequest)
    {
        var gameDto = _mapper.Map<UpdateGameRequest, GameDTO>(updateGameRequest);
        var updatedGameDto = await _gameService.UpdateGameAsync(gameDto);
        return Ok(updatedGameDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        await _gameService.DeleteGameAsync(id);
        return NoContent();
    }

    [HttpGet("/genre/{genreId:int}")]
    [Authorize(Roles = "Admin,User")]
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