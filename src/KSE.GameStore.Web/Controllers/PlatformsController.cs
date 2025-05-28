using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController(PlatformsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Platform>>> GetAll()
    {
        var platforms = await service.GetAllAsync();
        return Ok(platforms);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Platform>> GetById(int id)
    {
        var platform = await service.GetByIdAsync(id);
        if (platform is null)
            return NotFound();
        return Ok(platform);
    }

    [HttpPost]
    public async Task<ActionResult<Platform>> Create(Platform platform)
    {
        var created = await service.CreateAsync(platform);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Platform platform)
    {
        var updated = await service.UpdateAsync(id, platform);
        if (!updated)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}