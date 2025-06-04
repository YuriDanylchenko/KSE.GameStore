using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.Web.Requests.Platforms;
using KSE.GameStore.Web.Responses;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PlatformsController(IPlatformsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformResponse>>> GetAllPlatforms()
    {
        var platforms = await service.GetAllAsync();
        var dtos = platforms.Select(p => new PlatformResponse(p.Id, p.Name));
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlatformResponse>> GetByIdPlatforms(int id)
    {
        var platform = await service.GetByIdAsync(id);
        return Ok(new PlatformResponse(platform.Id, platform.Name));
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreatePlatforms(CreatePlatformRequest createPlatformRequest)
    {
        var id = await service.CreateAsync(createPlatformRequest.Name);
        return Ok(id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePlatforms(int id, UpdatePlatformRequest updatePlatformRequest)
    {
        var updated = await service.UpdateAsync(id, updatePlatformRequest.Name);
        if (!updated)
            return NotFound();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePlatforms(int id)
    {
        var deleted = await service.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return Ok();
    }
}