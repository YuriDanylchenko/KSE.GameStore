using AutoMapper;
using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.Web.Requests.Publishers;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PublishersController : ControllerBase
{
    private readonly IPublisherService _publisherService;
    private readonly IMapper _mapper;

    public PublishersController(IPublisherService publisherService, IMapper mapper)
    {
        _publisherService = publisherService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPublishers(int? pageNumber, int? pageSize)
    {
        var publisherDto = await _publisherService.GetAllPublishersAsync(pageNumber, pageSize);
        return Ok(publisherDto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPublisherById([FromRoute] int id)
    {
        var publisherDto = await _publisherService.GetPublisherByIdAsync(id);
        return Ok(publisherDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherRequest createPublisherRequest)
    {
        var publisherDto = _mapper.Map<CreatePublisherRequest, CreatePublisherDTO>(createPublisherRequest);
        var createdPublisherDto = await _publisherService.CreatePublisherAsync(publisherDto);
        return Ok(createdPublisherDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePublisher([FromBody] UpdatePublisherRequest updatePublisherRequest)
    {
        var publisherDto = _mapper.Map<UpdatePublisherRequest, UpdatePublisherDTO>(updatePublisherRequest);
        var updatedPublisherDto = await _publisherService.UpdatePublisherAsync(publisherDto);
        return Ok(updatedPublisherDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePublisher(int id)
    {
        await _publisherService.DeletePublisherAsync(id);
        return NoContent();
    }
}