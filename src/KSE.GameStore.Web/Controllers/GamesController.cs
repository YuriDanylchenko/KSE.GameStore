using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.ApplicationCore.Requests.Games;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGenres(int? pageNumber, int? pageSize)
    {
        var games = await _gameService.GetAllGamesAsync(pageNumber, pageSize);
        return Ok(games);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGameById(int id)
    {
        var game = await _gameService.GetGameByIdAsync(id);
        return Ok(game);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame(CreateGameRequest createGameRequest)
    {
        var createdGame = await _gameService.CreateGameAsync(createGameRequest);
        return Ok(createdGame);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGame(UpdateGameRequest updateGameRequest)
    {
        var updatedGame = await _gameService.UpdateGameAsync(updateGameRequest);
        return Ok(updatedGame);
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
        var game = await _gameService.GetGamesByGenreAsync(genreId);
        return Ok(game);
    }
}