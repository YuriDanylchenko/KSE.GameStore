using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.ApplicationCore.Requests.Platforms;
using KSE.GameStore.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController(IPlatformsService service) : ControllerBase
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
    public async Task<ActionResult<Platform>> Create(CreatePlatformRequest createPlatformRequest)
    {
        var created = await service.CreateAsync(createPlatformRequest.Name);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePlatformRequest updatePlatformRequest)
    {
        var updated = await service.UpdateAsync(id, updatePlatformRequest.Name);
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