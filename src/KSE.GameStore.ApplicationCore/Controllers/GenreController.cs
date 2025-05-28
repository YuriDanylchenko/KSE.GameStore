using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.ApplicationCore.Requests.Genre;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.ApplicationCore.Controllers;

[ApiController]
[Route("[controller]")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenreById([FromRoute] int id)
    {
        var genre = await _genreService.GetGenreByIdAsync(id);
        if (genre == null)
            return NotFound();
        
        return Ok(genre);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequest genreRequest)
    {
        var createdGenre = await _genreService.CreateGenreAsync(genreRequest.Name);
        if (createdGenre == null)
            return BadRequest("Genre could not be created");
        
        return Ok(createdGenre);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateGenre([FromBody] UpdateGenreRequest genreRequest)
    {
        var updatedGenre = await _genreService.UpdateGenreAsync(genreRequest.Id, genreRequest.Name);
        if (updatedGenre == null)
            return NotFound();
        
        return Ok(updatedGenre);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre([FromRoute] int id)
    {
        var isDeleted = await _genreService.DeleteGenreAsync(id);
        if (!isDeleted)
            return NotFound();
        
        return Ok(isDeleted);
    }
}
