using KSE.GameStore.ApplicationCore.Models;

namespace KSE.GameStore.ApplicationCore.Services;

public interface IPublisherService
{
    /// <summary>
    /// Retrieves a paginated list of all publishers.
    /// </summary>
    /// <param name="pageNumber">
    /// The 1-based page number to retrieve. If null, defaults to 1.
    /// </param>
    /// <param name="pageSize">
    /// The number of items per page. If null, defaults to 10.
    /// </param>
    /// <returns>
    /// A list of <see cref="PublisherDTO"/> representing the requested page of publishers.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when either parameter is less than or equal to zero.
    /// </exception>
    Task<List<PublisherDTO>> GetAllPublishersAsync(int? pageNumber, int? pageSize);
    
    /// <summary>
    /// Retrieves a publisher by its unique identifier with all related entities.
    /// </summary>
    /// <param name="id">The unique identifier of the publisher.</param>
    /// <returns>
    /// The complete <see cref="PublisherDTO"/> with the specified ID including name,
    /// website url, and description.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when id is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no publisher exists with the specified id.
    /// </exception>
    Task<PublisherDTO> GetPublisherByIdAsync(int id);
    
    /// <summary>
    /// Creates a new publisher.
    /// </summary>
    /// <param name="publisherDto">
    /// The <see cref="PublisherDTO"/> containing the publisher details.
    /// </param>
    /// <returns>
    /// The newly created <see cref="PublisherDTO"/>.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when:
    /// - A publisher with the same title already exists
    /// - Required fields are missing or invalid
    /// </exception>
    Task<PublisherDTO> CreatePublisherAsync(PublisherDTO publisherDto);
    
    /// <summary>
    /// Updates an existing publisher.
    /// </summary>
    /// <param name="publisherDto">
    /// The <see cref="PublisherDTO"/> containing updated publisher details.
    /// </param>
    /// <returns>
    /// The updated <see cref="PublisherDTO"/>.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when:
    /// - The new title conflicts with another publisher
    /// - Required fields are missing or invalid
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when:
    /// - The publisher to update doesn't exist
    /// </exception>
    Task<PublisherDTO> UpdatePublisherAsync(PublisherDTO publisherDto);
    
    /// <summary>
    /// Deletes a publisher by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the publisher to delete.</param>
    /// <returns>
    /// A task representing the asynchronous delete operation.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when id is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no publisher exists with the specified id.
    /// </exception>
    Task DeletePublisherAsync(int id);
}