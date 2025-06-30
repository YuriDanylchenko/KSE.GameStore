using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.Web.Requests.Genre;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetGenreById([FromRoute] int id)
    {
        var genre = await _genreService.GetGenreByIdAsync(id);
        return Ok(genre);
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAllGenres(int? pageNumber, int? pageSize)
    {
        var genre = await _genreService.GetAllGenresAsync(pageNumber, pageSize);
        return Ok(genre);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequest genreRequest)
    {
        var createdGenre = await _genreService.CreateGenreAsync(genreRequest.Name);
        return Ok(createdGenre);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateGenre([FromBody] UpdateGenreRequest genreRequest)
    {
        var updatedGenre = await _genreService.UpdateGenreAsync(genreRequest.Id, genreRequest.Name);
        return Ok(updatedGenre);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGenre([FromRoute] int id)
    {
        var isDeleted = await _genreService.DeleteGenreAsync(id);
        return Ok(isDeleted);
    }
}