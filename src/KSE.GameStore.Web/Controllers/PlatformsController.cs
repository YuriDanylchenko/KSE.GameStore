using KSE.GameStore.ApplicationCore.Domain;
using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.ApplicationCore.Requests.Platforms;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController(IPlatformsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformDto>>> GetAll()
    {
        var platforms = await service.GetAllAsync();
        var dtos = platforms.Select(p => new PlatformDto(p.Id, p.Name));
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlatformDto>> GetById(int id)
    {
        var platform = await service.GetByIdAsync(id);
        return Ok(new PlatformDto(platform.Id, platform.Name));
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreatePlatformRequest createPlatformRequest)
    {
        var id = await service.CreateAsync(createPlatformRequest.Name);
        return Ok(id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePlatformRequest updatePlatformRequest)
    {
        var updated = await service.UpdateAsync(id, updatePlatformRequest.Name);
        if (!updated)
            return NotFound();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return Ok();
    }
}