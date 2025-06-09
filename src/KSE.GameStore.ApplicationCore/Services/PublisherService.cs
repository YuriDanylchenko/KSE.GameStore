using AutoMapper;
using KSE.GameStore.ApplicationCore.Infrastructure;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace KSE.GameStore.ApplicationCore.Services;

public class PublisherService : IPublisherService
{
    private readonly IRepository<Publisher, int> _publisherRepository;
    private readonly ILogger<PublisherService> _logger;
    private readonly IMapper _mapper;    

    public PublisherService(IRepository<Publisher, 
        int> publisherRepository, 
        ILogger<PublisherService> logger, 
        IMapper mapper)
    {
        _publisherRepository = publisherRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<PublisherDTO>> GetAllPublishersAsync(int? pageNumber, int? pageSize)
    {
        if (pageNumber is <= 0)
            throw new BadRequestException($"Page number must be a positive integer. Provided: {pageNumber}");

        if (pageSize is <= 0)
            throw new BadRequestException($"Page size must be a positive integer. Provided: {pageSize}");

        var publisherEntities = await _publisherRepository.ListAsync(pageNumber ?? 1, pageSize ?? 10);

        return _mapper.Map<List<PublisherDTO>>(publisherEntities);
    }

    public async Task<PublisherDTO> GetPublisherByIdAsync(int id)
    {
        var publisherEntity = await _publisherRepository.GetByIdAsync(id);

        if (publisherEntity == null)
        {
            _logger.LogNotFound($"publisher/{id}");
            throw new NotFoundException($"Publisher with ID {id} not found.");
        }

        return _mapper.Map<PublisherDTO>(publisherEntity);
    }

    public async Task<PublisherDTO> CreatePublisherAsync(PublisherDTO publisherDto)
    {
        var existing = await _publisherRepository
            .ListAsync(g => g.Name.ToLower() == publisherDto.Name.ToLower());

        if (existing.Any())
            throw new BadRequestException($"A publisher with the name '{publisherDto.Name}' already exists.");

        var publisher = new Publisher
        {
            Name = publisherDto.Name,
            WebsiteUrl = publisherDto.WebsiteUrl,
            Description = publisherDto.Description
        };

        await _publisherRepository.AddAsync(publisher);
        await _publisherRepository.SaveChangesAsync();

        return publisherDto;
    }

    public async Task<PublisherDTO> UpdatePublisherAsync(PublisherDTO publisherDto)
    {
        var publisherEntity = await _publisherRepository.GetByIdAsync(publisherDto.Id);

        if (publisherEntity == null)
        {
            _logger.LogNotFound($"publisher/{publisherDto.Id}");
            throw new NotFoundException($"Publisher with ID {publisherDto.Id} not found.");
        }
        
        var nameTaken = (await _publisherRepository
                .ListAsync(p => p.Id != publisherDto.Id &&
                                p.Name.Equals(publisherDto.Name, StringComparison.OrdinalIgnoreCase)))
                .Any();
        
        if (nameTaken)
            throw new BadRequestException($"Publisher '{publisherDto.Name}' already exists.");
        
        publisherEntity.Name = publisherDto.Name;

        // If description is null, it means that user don't want to change it
        if (publisherDto.Description != null)
        {
            publisherEntity.Description = publisherDto.Description;
        }

        // If website url is null, it means that user don't want to change it
        if (publisherDto.WebsiteUrl != null)
        {
            publisherEntity.WebsiteUrl = publisherDto.WebsiteUrl;
        }
        
        _publisherRepository.Update(publisherEntity);
        await _publisherRepository.SaveChangesAsync();

        return _mapper.Map<PublisherDTO>(_publisherRepository.GetByIdAsync(publisherDto.Id));
    }

    public async Task DeletePublisherAsync(int id)
    {
        var publisherEntity = await _publisherRepository.GetByIdAsync(id);

        if (publisherEntity == null)
        {
            _logger.LogNotFound($"publisher/{id}");
            throw new NotFoundException($"Publisher with ID {id} not found.");
        }
        
        _publisherRepository.Delete(publisherEntity);
        await _publisherRepository.SaveChangesAsync();
    }
}